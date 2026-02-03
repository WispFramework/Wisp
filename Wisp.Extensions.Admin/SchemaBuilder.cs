using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Extensions.Admin.Attributes;
using Wisp.Extensions.Admin.Data;

namespace Wisp.Extensions.Admin;

public class SchemaBuilder(ILogger<SchemaBuilder> log, IServiceProvider serviceProvider, AdminConfigBuilder.AdminConfig config)
{
    public async Task<FormSchema> Build(Type type, string title, string action, string method)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService(config.DbContextType) as DbContext;
        
        var schema = new FormSchema
        {
            Title = title,
            Action = action,
            Fields = [],
            Method = method,
        };
        
        var props = type.GetProperties()
            .Where(p => p is { CanRead: true, CanWrite: true }).ToList();

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<NotMappedAttribute>() != null || prop.GetCustomAttribute<IgnoreFieldAttribute>() != null) continue;
            
            if(prop.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase) && schema.Method != "PATCH") continue;
            
            var field = new FormField
            {
                ValueType = prop.PropertyType,
                Name = prop.Name,
                Label = prop.Name,
                FieldType = GetFieldType(prop)
            };

            if (prop.PropertyType is
                {
                    IsEnum: false,
                    IsValueType: false,
                    IsAbstract: false,
                    IsInterface: false
                } 
                && prop.PropertyType != typeof(string)
                && dbContext is not null)
            {
                var valueSourceAttr = prop.GetCustomAttribute<ValueSourceAttribute>();
                
                var sourceName = valueSourceAttr?.Name ?? 
                                 (prop.PropertyType.IsGenericType? 
                                     Util.Pluralize(prop.PropertyType.GenericTypeArguments[0].Name) :
                                     Util.Pluralize(prop.PropertyType.Name));
                
                log.LogDebug("Property {Name} has a source attribute: {SourceName}", prop.Name, sourceName);
                var setProp = dbContext.GetType().GetProperty(sourceName);
                if (setProp != null)
                {
                    log.LogDebug("Property {Name} has a set property", prop.Name);
                    
                    var propTypeName = prop.PropertyType.IsGenericType? prop.PropertyType.GetGenericArguments()[0].Name : prop.PropertyType.Name;
                    log.LogDebug("Prop type name is {Name}", propTypeName);
                    var items = await GetTableAsListAsync(dbContext, propTypeName);
                    log.LogDebug("Found {N} items", items.Count);
                    field.Values = items.Select(i => (i.ToString() ?? "", i.Id.ToString() as object)).ToList();
                }
            }
            
            var labelAttr = prop.GetCustomAttribute<FieldLabelAttribute>();
            if(!string.IsNullOrEmpty(labelAttr?.Label)) field.Label = labelAttr.Label;

            var defaultValueAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
            if(defaultValueAttr != null) field.DefaultValue = defaultValueAttr.Value;

            if (prop.PropertyType.IsEnum)
            {
                var values = Enum.GetNames(prop.PropertyType)
                    .Zip(Enum.GetValues(prop.PropertyType).Cast<object>(), (name, value) => (name, value))
                    .ToList();
                
                field.Values = values;
            }

            var disabledAttr = prop.GetCustomAttribute<DisabledFieldAttribute>();
            if (disabledAttr is not null) field.Disabled = true;
            
            schema.Fields.Add(field);
        }

        return schema;
    }

    private FieldType GetFieldType(PropertyInfo prop)
    {
        var typeAttr = prop.GetCustomAttribute<FieldTypeAttribute>();
        if (typeAttr != null) return typeAttr.Type;

        return prop.PropertyType switch
        {
            var t when t == typeof(int) ||
                       t == typeof(long) ||
                       t == typeof(short) ||
                       t == typeof(float) ||
                       t == typeof(double) ||
                       t == typeof(decimal) => FieldType.Number,
            var t when t.IsArray || 
                       t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>) => FieldType.MultiSelect,
            { IsEnum: true } => FieldType.Select,
            var t when
                t == typeof(DateTime) ||
                t == typeof(DateTimeOffset) => FieldType.Date,
            var t when !t.IsValueType && t is { IsEnum: false, IsArray: false, IsGenericType: false } && t != typeof(string) => FieldType.Select,
            _ => FieldType.Text
        };
    }
    
    public static async Task<List<CrudModel>> GetTableAsListAsync(DbContext dbContext, string entityName)
    {
        // 1. Find the entity Type
        var entityType = dbContext.GetType().Assembly.GetTypes()
            .FirstOrDefault(t => t.Name == entityName);

        if (entityType == null)
            throw new InvalidOperationException($"Type '{entityName}' not found.");

        if (!typeof(CrudModel).IsAssignableFrom(entityType))
            throw new InvalidOperationException($"Type '{entityName}' does not inherit from CrudModel.");

        // 2. Get DbSet<TEntity> via reflection
        MethodInfo setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)
            .MakeGenericMethod(entityType);
        var dbSet = setMethod.Invoke(dbContext, null); // DbSet<TEntity>

        // Get all static public methods called ToListAsync
        MethodInfo toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "ToListAsync" && m.IsGenericMethodDefinition)
            .FirstOrDefault(m => 
            {
                var parameters = m.GetParameters();
                return parameters.Length >= 1 
                       && typeof(IQueryable<>).IsAssignableFrom(
                           parameters[0].ParameterType.GetGenericTypeDefinition());
            });

        if (toListAsyncMethod == null)
            throw new InvalidOperationException("Could not find ToListAsync method");

        // Make it generic
        toListAsyncMethod = toListAsyncMethod.MakeGenericMethod(entityType);

        // 4. Call the extension method (static) passing DbSet<T> as first arg
        var task = (Task)toListAsyncMethod.Invoke(null, new object[] { dbSet, null });

        await task;

        // 5. Get Result
        var resultProperty = task.GetType().GetProperty("Result");
        var typedList = resultProperty.GetValue(task) as System.Collections.IEnumerable;

        var entityBaseList = new List<CrudModel>();
        foreach (CrudModel item in typedList)
            entityBaseList.Add(item);

        return entityBaseList;
    }
}