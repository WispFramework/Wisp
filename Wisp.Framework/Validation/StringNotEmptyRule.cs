// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Validation;

public class StringNotEmptyRule : IValidationRule
{
    public ValidationResult Validate(object? value, string error)
    {
        if (value is not string str)
        {
            return new ValidationResult
            {
                Ok = false,
                Error = "not a string"
            };
        }
        
        var ok = !string.IsNullOrWhiteSpace(str);
        return new ValidationResult
        {
            Ok = ok,
            Error = (ok)? "" : error
        };
    }
}