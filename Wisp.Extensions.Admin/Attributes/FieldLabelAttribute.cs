namespace Wisp.Extensions.Admin.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FieldLabelAttribute(string label) : Attribute
{
    public string Label => label;
}