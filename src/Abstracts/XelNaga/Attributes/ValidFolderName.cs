using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Aiursoft.XelNaga.Attributes;

public class ValidFolderName : TestableValidationAttribute
{
    private char[] GetInvalidFileNameChars()
    {
        var systemInvalid = Path.GetInvalidFileNameChars().ToList();
        systemInvalid.Add('*');
        return systemInvalid.ToArray();
    }

    public override bool IsValid(object value)
    {
        if (value is string val)
        {
            return
                val.IndexOfAny(GetInvalidFileNameChars()) < 0 &&
                !string.IsNullOrWhiteSpace(val);
        }

        return false;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (IsValid(value))
        {
            return ValidationResult.Success;
        }

        if (string.IsNullOrWhiteSpace(value as string))
        {
            return new ValidationResult("Empty string is not a valid file name!");
        }

        var invalidCharacters = string.Empty;
        foreach (var invalidChar in GetInvalidFileNameChars())
        {
            if (value is string val && val.Contains(invalidChar))
            {
                invalidCharacters += $" '{invalidChar}',";
            }
        }

        return new ValidationResult(
            $"The {validationContext.DisplayName} can not contain invalid characters{invalidCharacters.TrimEnd(',')}!");
    }
}