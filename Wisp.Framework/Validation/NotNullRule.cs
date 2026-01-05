// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Validation;

public class NotNullRule : IValidationRule
{
    public ValidationResult Validate(object? value, string error)
    {
        return new ValidationResult
        {
            Ok = (value is not null),
            Error = (value is not null)? "" : error
        };
    }
}