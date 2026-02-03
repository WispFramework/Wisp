using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Extensions.Admin.Data;
using Wisp.Extensions.Admin.Generators;
using Wisp.Framework;
using Wisp.Framework.Extensions;
using Wisp.Framework.Http;

namespace Wisp.Extensions.Admin.AdminSiteGenerator;

public class ActionMapper
{
    private readonly Router _router;
    private readonly SchemaBuilder _schemaBuilder;
    private readonly ILogger<ActionMapper> _log;
    private readonly AdminConfigBuilder.AdminConfig _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly ListViewRenderer _listViewRenderer;

    public ActionMapper(Router router,
        SchemaBuilder schemaBuilder,
        ILogger<ActionMapper> log, 
        AdminConfigBuilder.AdminConfig config,
        IServiceProvider serviceProvider,
        ListViewRenderer listViewRenderer)
    {
        _router = router;
        _schemaBuilder = schemaBuilder;
        _log = log;
        _config = config;
        _serviceProvider = serviceProvider;
        _listViewRenderer = listViewRenderer;
    }
    
    public void MapAll(List<Type> types)
    {
        _log.LogInformation("Mapping Admin routes for {Count} types", types.Count);
        
        foreach (var type in types)
        {
            var subName = Util.Pluralize(type.Name);
            
            _log.LogInformation("Adding admin route '[GET] /Admin/{SubName}' for {Type}", subName, type.Name);
            _router.Add("GET", $"/Admin/{subName}", async context => await GetListHandler(context, type, subName, types));
            
            _log.LogInformation("Adding admin route '[GET] /Admin/{SubName}/New' for {Type}", subName, type.Name);
            _router.Add("GET", $"/Admin/{subName}/New", async context => await GetCreateHandler(context, type, subName));
            
            _log.LogInformation("Adding admin route '[POST] /Admin/{SubName}/New' for {Type}", subName, type.Name);
            _router.Add("POST", $"/Admin/{subName}/New", async context => await PostCreateHandler(context, type, subName));
        }
    }

    private async Task GetCreateHandler(IHttpContext context, Type type, string subName)
    {
        var schema = await _schemaBuilder.Build(type, type.Name, $"/Admin/{subName}/New", "POST");
        var form = FormBuilder.Build(schema, "POST");
                
        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.Body = new MemoryStream(form.AsUtf8Bytes());
    }

    public async Task GetListHandler(IHttpContext context, Type type, string subName, List<Type> allTypes)
    {
        var items = await GetAllWithIncludesAsync(type);

        // var json =  JsonSerializer.Serialize(items);
        if (items is IList list)
        {
            var rendered = _listViewRenderer.Render(type, list, subName, allTypes);
        
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Body = new MemoryStream(rendered.AsUtf8Bytes());    
        }
        else
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Body = new MemoryStream("notfound".AsUtf8Bytes());
        }
    }

    private async Task PostCreateHandler(IHttpContext context, Type type, string subName)
    {
        var formData = context.Request.FormData;
        var schema = await _schemaBuilder.Build(type, type.Name, $"/Admin/{subName}/New", "POST");
        
        var instance =  Activator.CreateInstance(type);
        if (instance is null)
            throw new InvalidOperationException($"Cannot create instance of {type.Name}");

        foreach (var field in schema.Fields)
        {
            var targetProp = instance.GetType().GetProperty(field.Name);
            if (targetProp is null) continue;
            
            switch (field.FieldType)
            {
                case FieldType.Text:
                case FieldType.TextArea:
                case FieldType.Password:
                case FieldType.Color:
                    if (formData.TryGetValue(field.Name, out var value))
                    {
                        targetProp.SetValue(instance, value);
                    }

                    break;
                case FieldType.Date:
                    if (formData.TryGetValue(field.Name, out var dateString))
                    {
                        var date = TryParseDateTime(dateString);
                        if(date is not null) targetProp.SetValue(instance, date);
                    }
                    break;
                case FieldType.Number:
                    if (formData.TryGetValue(field.Name, out var numberString))
                    {
                        targetProp.SetValue(instance, targetProp.PropertyType switch
                        {
                            var t when t == typeof(int) => int.Parse(numberString),
                            var t when t == typeof(long) => long.Parse(numberString),
                            var t when t == typeof(short) => short.Parse(numberString),
                            var t when t == typeof(float) => float.Parse(numberString),
                            var t when t == typeof(double) => double.Parse(numberString),
                            var t when t == typeof(decimal) => decimal.Parse(numberString),
                            _ => throw new ArgumentException($"can't deserialize a numeric type info {targetProp.PropertyType.Name}")
                        });
                    }
                    break;
                case FieldType.Select:
                    if (formData.TryGetValue(field.Name, out var selectString))
                    {
                        var obj = await TryGetForeignKeyItem(targetProp.PropertyType, selectString);
                        if (obj is not null) targetProp.SetValue(instance, obj);
                    }
                    break;
                case FieldType.MultiSelect:
                    if (formData.TryGetValue(field.Name, out var multiSelectString))
                    {
                        var split = multiSelectString.Split(',');
                        if(split.Length == 0) continue;
                        var obj = await TryGetForeignKeyItems(targetProp.PropertyType, split.ToList());
                        targetProp.SetValue(instance, obj);
                    }

                    break;
            }
        }

        await InsertEntity(type, instance);
        
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Body = new MemoryStream("ok".AsUtf8Bytes());
    }

    private async Task<object?> TryGetForeignKeyItem(Type type, string selectString)
    {
        if (!Guid.TryParse(selectString, out var guid)) return null;
        
        await using var scope = _serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService(_config.DbContextType) as DbContext;
        
        var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)
            .MakeGenericMethod(type);
        var dbSet = setMethod.Invoke(db, null);

        var firstAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "FirstOrDefaultAsync")
            .Where(m => m.IsGenericMethodDefinition)
            .First(m =>
            {
                var p = m.GetParameters();

                return p.Length == 3
                       && p[0].ParameterType.IsGenericType
                       && p[1].ParameterType.IsGenericType
                       && p[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                       && p[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>);
            })
            .MakeGenericMethod(type);

        var param = Expression.Parameter(type, "i");
        var idProp = Expression.Property(param, "Id");
        
        var body = Expression.Equal(idProp, Expression.Constant(guid));
        
        var lambdaType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
        var lambda = Expression.Lambda(lambdaType, body, param);
        
        var task = (Task)firstAsync.Invoke(null, [dbSet, lambda, CancellationToken.None]);
        await task;
        
        var result = task.GetType()
            .GetProperty("Result")
            .GetValue(task);

        return result;
    }
    
    private async Task<IList> TryGetForeignKeyItems(Type type, List<string> selectStrings)
    {
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
            throw new InvalidOperationException("Expected List<TEntity>");

        var entityType = type.GetGenericArguments()[0];

        var ids = selectStrings
            .Select(s => Guid.TryParse(s, out var g) ? g : (Guid?)null)
            .Where(g => g != null)
            .Select(g => g.Value)
            .ToList();

        if (ids.Count == 0)
            return new List<object?>();

        await using var scope = _serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService(_config.DbContextType) as DbContext;

        var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)!
            .MakeGenericMethod(entityType);

        var dbSet = setMethod.Invoke(db, null);

        // Where<TEntity>(IQueryable<TEntity>, Expression<Func<TEntity,bool>>)
        var whereMethod = typeof(Queryable)
            .GetMethods()
            .First(m =>
                m.Name == "Where"
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType);

        // ToListAsync<TEntity>(IQueryable<TEntity>, CancellationToken)
        var toListAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "ToListAsync")
            .Where(m => m.IsGenericMethodDefinition)
            .First(m =>
            {
                var p = m.GetParameters();
                return p.Length == 2
                       && p[0].ParameterType.IsGenericType
                       && p[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>);
            })
            .MakeGenericMethod(entityType);

        // i => ids.Contains(i.Id)
        var param = Expression.Parameter(entityType, "i");
        var idProp = Expression.Property(param, "Id");

        var containsMethod = typeof(List<Guid>)
            .GetMethod(nameof(List<Guid>.Contains), new[] { typeof(Guid) });

        var idListExpr = Expression.Constant(ids);
        var containsCall = Expression.Call(idListExpr, containsMethod!, idProp);

        var lambdaType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));
        var lambda = Expression.Lambda(lambdaType, containsCall, param);

        // Apply Where(...)
        var filtered = whereMethod.Invoke(null, new object[] { dbSet, lambda });

        // Execute ToListAsync(...)
        var task = (Task)toListAsync.Invoke(null, new object[]
        {
            filtered,
            CancellationToken.None
        });

        await task;

        var result = task.GetType().GetProperty("Result")!.GetValue(task)
            as System.Collections.IEnumerable;

        // Create List<TEntity>
        var listType = typeof(List<>).MakeGenericType(entityType);
        var typedList = (System.Collections.IList)Activator.CreateInstance(listType)!;

        foreach (var item in result!)
            typedList.Add(item);

        return typedList;
    }

    private async Task InsertEntity(Type type, object instance)
    {
        if (!type.IsInstanceOfType(instance))
            throw new ArgumentException("Entity istance does not match type");
        
        await using var scope = _serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService(_config.DbContextType) as DbContext;

        var setMethod = typeof(DbContext)
            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
            .MakeGenericMethod(type);
        
        var dbSet = setMethod.Invoke(db, null);

        var addMethod = dbSet?.GetType()
            .GetMethod(nameof(DbSet<object>.Add), [type])!;
        
        addMethod.Invoke(dbSet, [ instance ]);

        await db.SaveChangesAsync();
    }
    
    public async Task<object> GetAllWithIncludesAsync(Type entityType)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService(_config.DbContextType) as DbContext;
        
        // DbSet<TEntity>
        var setMethod = typeof(DbContext)
            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
            .MakeGenericMethod(entityType);
        var dbSet = setMethod.Invoke(db, null); // DbSet<TEntity>

        // IQueryable<TEntity>
        var queryable = dbSet;

        // Get all navigation properties
        var navProps = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p =>
                !p.PropertyType.IsValueType &&
                p.PropertyType != typeof(string) &&
                (typeof(IEnumerable<CrudModel>).IsAssignableFrom(p.PropertyType) ||
                 typeof(CrudModel).IsAssignableFrom(p.PropertyType)))
            .ToList();

        // Include each navigation
        foreach (var nav in navProps)
        {
            var includeMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m =>
                    m.Name == "Include" &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .MakeGenericMethod(entityType, nav.PropertyType);

            // Build lambda: i => i.NavProperty
            var param = Expression.Parameter(entityType, "i");
            var body = Expression.Property(param, nav);
            var lambdaType = typeof(Func<,>).MakeGenericType(entityType, nav.PropertyType);
            var lambda = Expression.Lambda(lambdaType, body, param);

            queryable = includeMethod.Invoke(null, new object[] { queryable, lambda });
        }

        // ToListAsync<TEntity>(IQueryable<TEntity>, CancellationToken)
        var toListAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "ToListAsync")
            .Where(m => m.IsGenericMethodDefinition)
            .First(m =>
            {
                var p = m.GetParameters();
                return p.Length == 2
                    && p[0].ParameterType.IsGenericType
                    && p[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>);
            })
            .MakeGenericMethod(entityType);

        var task = (Task)toListAsync.Invoke(null, new object[] { queryable, CancellationToken.None });
        await task;

        var result = task.GetType().GetProperty("Result")!.GetValue(task);
        return result; // List<TEntity> with included navs
    }

    private DateTime? TryParseDateTime(string input)
    {
        if(DateTime.TryParse(input, out var o)) return o;
        return null;
    }
}