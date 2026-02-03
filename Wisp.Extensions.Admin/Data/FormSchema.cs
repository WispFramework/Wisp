namespace Wisp.Extensions.Admin.Data;

public class FormSchema
{
    public List<FormField> Fields { get; set; }
    
    public string Title { get; set; }
    
    public string Action { get; set; }
    
    public string Method { get; set; }
}