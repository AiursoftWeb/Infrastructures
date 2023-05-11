using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDKTools.Attributes;

public class NoSpace : TestableValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is string val) return !val.Contains(" ") && !val.Contains("\r") && !val.Contains("\n");
        return false;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (IsValid(value))
            return ValidationResult.Success;
        return new ValidationResult($"The {validationContext.DisplayName} can not contain space!");
    }
}