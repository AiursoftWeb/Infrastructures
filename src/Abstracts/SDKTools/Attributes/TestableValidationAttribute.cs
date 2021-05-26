using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.SDKTools.Attributes
{
    public class TestableValidationAttribute : ValidationAttribute
    {
        public ValidationResult TestEntry(object value)
        {
            return IsValid(value, new ValidationContext(value) { DisplayName = "Mock-display-name" });
        }
    }
}
