using System.ComponentModel.DataAnnotations;

namespace Aiursoft.CSTools.Attributes;

public class TestableValidationAttribute : ValidationAttribute
{
    public ValidationResult TestEntry(object value)
    {
        return IsValid(value, new ValidationContext(value) { DisplayName = "Mock-display-name" });
    }
}