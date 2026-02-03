namespace Wisp.Extensions.Admin.Data;

public class FormField
{
    public Type ValueType { get; set; }
    
    public string Name { get; set; }
    
    public string Label { get; set; }
    
    public object? DefaultValue { get; set; }
    
    public FieldType FieldType { get; set; }
    
    public List<(string, object)>? Values { get; set; }
    
    public bool Disabled { get; set; }
}