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