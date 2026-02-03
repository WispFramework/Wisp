namespace Wisp.Extensions.Admin.Attributes;

public class ValueSourceAttribute(string name) : Attribute
{
    public string Name => name;
}