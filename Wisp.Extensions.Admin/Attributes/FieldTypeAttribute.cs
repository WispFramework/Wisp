using Wisp.Extensions.Admin.Data;

namespace Wisp.Extensions.Admin.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FieldTypeAttribute(FieldType type) : Attribute
{
    public FieldType Type => type;
}