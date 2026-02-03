using System.Collections;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Wisp.Extensions.Admin.Attributes;
using Wisp.Extensions.Admin.Data;

namespace Wisp.Extensions.Admin.Generators;

public class ListViewRenderer(ILogger<ListViewRenderer> log)
{
    public string Render(Type type, IList items, string subName, List<Type> allTypes)
    {
        if (!typeof(ICrudModel).IsAssignableFrom(type))
            throw new ArgumentException($"The type {type} must extend ICrudModel to work here");

        var navItems =
            allTypes.Where(typeof(ICrudModel).IsAssignableFrom)
                .Select(t => Util.Pluralize(t.Name))
                .Select(i => (i, $"/Admin/{i}"))
                .ToList();
        
        var columns = new List<string>() { "Id" };

        var columnsAttr = type.GetCustomAttribute<AdminColumnsAttribute>();
        if (columnsAttr is not null)
        {
            log.LogDebug("Additional columns found: {Columns}", string.Join(',', columnsAttr.ColumnNames));
            columns.AddRange(columnsAttr.ColumnNames);
        }
        
        columns.Add("");

        var sb = new StringBuilder();
        sb.Append("<p>");
        foreach (var item in navItems)
        {
            sb.Append($"<a href='{item.Item2}'>{item.i}</a> ");
        }
        sb.AppendLine("</p>");
        sb.AppendLine($"<h1>{subName}</h1>");
        sb.AppendLine($"<p><a href='/Admin/{subName}/New'>Create New</a></p>");
        sb.AppendLine("<table border='1' style='width: 100%'>");

        sb.AppendLine("<thead><tr>");

        foreach (var col in columns)
        {
            sb.AppendLine($"<th>{col}</th>");
        }
        
        sb.AppendLine("</tr></thead>");
        sb.AppendLine("<tbody>");

        if (items is not null)
        {
            foreach (var item in items)
            {
                if (item is ICrudModel m)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine($"<td><a href='/Admin/{subName}/{m.Id}'>{m.Id}</a></td>");
                    foreach (var col in m.GetColumnValues())
                    {
                        sb.AppendLine($"<td>{col}</td>");
                    }
                    sb.AppendLine($"<td><a href='/Admin/{subName}/{m.Id}/Edit'>Edit</a> <a href='/Admin/{subName}/{m.Id}/Delete' style='color: red;'>Delete</a></td>");
                    sb.AppendLine("</tr>");
                }
            }   
        }
        
        sb.AppendLine("</tbody>");
        sb.AppendLine("</table>");

        return sb.ToString();
    }
}