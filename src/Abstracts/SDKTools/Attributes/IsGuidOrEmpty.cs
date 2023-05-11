using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDKTools.Attributes;

public class IsGuidOrEmpty : TestableValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is not string val)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(val) || Guid.TryParse(val, out _);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (IsValid(value))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"The {validationContext.DisplayName} is not a valid GUID value!");
    }
}