using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models.PasswordViewModels
{
    public class ForgotPasswordViaEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
