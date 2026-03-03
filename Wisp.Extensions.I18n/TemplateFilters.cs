using System.Text.Json;
using Fluid;
using Fluid.Values;
using Wisp.Framework.Views;

namespace Wisp.Extensions.I18n;

public class TemplateFilters(StringTranslator translator)
{
    public ValueTask<FluidValue> GetText(FluidValue input, FilterArguments args, TemplateContext context)
    {

        if (context.Model.ToObjectValue() is not ViewModel vm)
        {
            return new StringValue("");
        }
        
        Console.WriteLine(JsonSerializer.Serialize(vm.Middleware));
        
        var stringValue = input.ToStringValue();
        if (string.IsNullOrEmpty(stringValue))
        {
            return new StringValue("");
        }

        if (string.IsNullOrWhiteSpace(vm.Locale))
        {
            return new StringValue(stringValue);
        }
        
        var translation = translator.GetTranslation(vm.Locale.ToLowerInvariant(), stringValue);
        return new StringValue(translation);
    }
}