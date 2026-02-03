namespace Wisp.Extensions.Admin.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AdminColumnsAttribute(string[] columnNames) : Attribute
{
    public string[] ColumnNames => columnNames;
}