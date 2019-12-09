using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Aiursoft.Pylon.Attributes
{
    public class ValidFolderName : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string val)
            {
                return val.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
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
                var invalidCharators = string.Empty;
                foreach (var invalidChar in Path.GetInvalidFileNameChars())
                {
                    if (value is string val && val.Contains(invalidChar))
                    {
                        invalidCharators += $" '{invalidChar}',";
                    }
                }
                return new ValidationResult($"The {validationContext.DisplayName} can not contain invalid charactor{invalidCharators.TrimEnd(',')}!");
            }
        }
    }
}
