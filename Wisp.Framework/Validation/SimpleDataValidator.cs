// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Validation;

public class SimpleDataValidator
{
    private readonly List<ValidationItem> _items = new List<ValidationItem>();

    public ValidationItemBuilder For(object? value, string name)
    {
        return new ValidationItemBuilder(value, name, this);
    }

    public class ValidationItemBuilder(object? value, string name, SimpleDataValidator validator)
    {
        public SimpleDataValidator NotNull()
        {
            validator._items.Add(new ValidationItem(name, typeof(NotNullRule), value, $"The field {name} is invalid because it is null"));
            return validator;
        }

        public SimpleDataValidator StringNotEmpty()
        {
            validator._items.Add(new ValidationItem(name, typeof(StringNotEmptyRule), value, $"The field {name} is invalid because it is empty"));
            return validator;
        }
    }

    public void Validate(Action ok, Action<List<string>> fail)
    {
        var results = new List<ValidationResult>();
        foreach (var val in _items)
        {
            var instance = Activator.CreateInstance(val.Rule) as IValidationRule;
            if(instance is null) continue;
            results.Add(instance.Validate(val.Value, val.ErrorMessage));
        }

        if (results.Any(r => !r.Ok))
        {
            fail.Invoke(results.Where(r => r.Error != null).Select(r => r.Error ?? "").ToList());
        }
        else
        {
            ok.Invoke();
        }
    }

    public void Validate(Action ok) => Validate(ok, errors => { });
    
    public void Validate(Action<List<string>> fail) => Validate(() => { }, fail);

    public List<string> Validate()
    {
        var results = new List<string>();
        Validate(errors => { results.AddRange(errors); });
        return results;
    }
    
    public record ValidationItem(string Name, Type Rule, object? Value, string ErrorMessage);
}