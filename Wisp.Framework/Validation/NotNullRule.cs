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