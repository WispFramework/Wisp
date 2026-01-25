using System.Text.Json;
using Fluid;
using Fluid.Values;

namespace Wisp.Framework.Views;

public static class FluidExtensions
{
    public static ValueTask<FluidValue> ToJson(FluidValue input, FilterArguments args, TemplateContext context)
    {
        var obj = input.ToObjectValue();
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions {  WriteIndented = true });
        return new StringValue(json);
    }
    
    public static ValueTask<FluidValue> DateToAgo(FluidValue input, FilterArguments args, TemplateContext context)
    {
        if (string.IsNullOrEmpty(input.ToStringValue())) return new StringValue("");
        var dateObj = input.ToObjectValue();
        if (dateObj is null) return new StringValue("");

        DateTime? date = dateObj switch
        {
            DateTime d => d,
            DateTimeOffset dof => dof.DateTime,
            _ => null
        };

        if (date is null) return new StringValue("invalid date");
        if(date.Value.Kind != DateTimeKind.Utc) date = date.Value.ToUniversalTime();

        var now = DateTime.UtcNow;
        var ago = now - date.Value;

        var total = 0.0;
        var suffix = "";

        if (ago.TotalDays > 365)
        {
            total = ago.TotalDays / 365;
            suffix = "year";
        }
        else if (ago.TotalDays > 30)
        {
            total = ago.TotalDays / 30;
            suffix = "month";
        }
        else if (ago.TotalDays > 7)
        {
            total = ago.TotalDays / 7;
            suffix = "week";
        }
        else if (ago.TotalHours > 24)
        {
            total = ago.TotalHours / 24;
            suffix = "day";
        }
        else if (ago.TotalMinutes > 60)
        {
            total = ago.TotalHours;
            suffix = "hour";
        }
        else if (ago.TotalSeconds > 60)
        {
            total = ago.TotalMinutes;
            suffix = "minute";
        }
        else
        {
            total = ago.TotalSeconds;
            suffix = "second";
        }

        if (total >= 2) suffix += "s";

        return new StringValue($"{total:F0} {suffix}");
    }
}