using System.Text;
using Wisp.Extensions.Admin.Data;

namespace Wisp.Extensions.Admin.Generators;

public static class FormBuilder
{
    public static string Build(FormSchema schema, string method)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<form action='{schema.Action}' method='{method}'>");

        sb.AppendLine($"<h2>{schema.Title}</h2>");

        foreach (var field in schema.Fields)
        {
            sb.AppendLine("<fieldset>");

            sb.AppendLine($"<legend>{field.Label}</legend>");

            var disabled = (
                field.Name.Equals("id",  StringComparison.InvariantCultureIgnoreCase) ||
                field.Disabled)
                ? "disabled" 
                : "";
            
            switch (field.FieldType)
            {
                case FieldType.Text:
                    sb.AppendLine($"<input type='text' name='{field.Name}' placeholder='{field.Label}' value='{field.DefaultValue}' {disabled} />");
                    break;
                case FieldType.Password:
                    sb.AppendLine($"<input type='password' name='{field.Name}' placeholder='{field.Label}' value='{field.DefaultValue}' {disabled} />");
                    break;
                case FieldType.Number:
                    sb.AppendLine($"<input type='number' name='{field.Name}' placeholder='{field.Label}' value='{field.DefaultValue}' {disabled} />");
                    break;
                case FieldType.Date:
                    sb.AppendLine($"<input type='date' name='{field.Name}' placeholder='{field.Label}' value='{field.DefaultValue}' {disabled} />");
                    break;
                case FieldType.Select:
                case FieldType.MultiSelect:
                    sb.AppendLine($"<select {disabled} name='{field.Name}' {(field.FieldType == FieldType.MultiSelect? "multiple" : "")}>");
                    if (field.Values is not null)
                    {
                        foreach (var (name, val) in field.Values)
                        {
                            sb.AppendLine($"<option value='{val}'>{name}</option>");
                        }   
                    }
                    sb.AppendLine($"</select>");
                    break;
                case FieldType.TextArea:
                    sb.AppendLine($"<textarea {disabled} name='{field.Name}'>{field.DefaultValue}</textarea>");
                    break;
                case FieldType.Checkbox:
                    sb.AppendLine($"<checkbox {disabled} name='{field.Name}' value='{field.DefaultValue}' />");
                    break;
                case FieldType.Radio:
                    if (field.Values is not null)
                    {
                        foreach (var (name, val) in field.Values)
                        {
                            sb.AppendLine($"<radio {disabled} name='{field.Name}' value='{val}'>{name}</option>");
                        }   
                    }
                    break;
                case FieldType.Color:
                    sb.AppendLine($"<input {disabled} type='color' name='{field.Name}' placeholder='{field.Label}' value='{field.DefaultValue}' />");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.AppendLine($"</fieldset>");
        }
        
        sb.AppendLine("<button>Submit</button>");
        sb.AppendLine("</form>");

        return sb.ToString();
    }
}