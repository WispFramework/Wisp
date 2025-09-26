namespace Wisp.Framework.Validation;

public interface IValidationRule
{
    ValidationResult Validate(object? value, string error);
}