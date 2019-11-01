using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Attributes
{
    public class IsGuid : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string val)
            {
                return Guid.TryParse(val, out _);
            }
            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsValid(value))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"The {validationContext.DisplayName} is not a valid GUID value!");
            }
        }
    }
}
