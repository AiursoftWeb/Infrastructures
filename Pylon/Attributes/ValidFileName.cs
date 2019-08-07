using Aiursoft.Pylon.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Aiursoft.Pylon.Attributes
{
    public class ValidFolderName : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string val)
            {
                return !val.Any(t => StringOperation.InvalidFileAndFolderNames.Contains(t));
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
                return new ValidationResult($"The {validationContext.DisplayName} can not contain invalid charactor!");
            }
        }
    }
}
