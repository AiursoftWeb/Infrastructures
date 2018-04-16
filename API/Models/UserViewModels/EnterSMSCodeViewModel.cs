using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models.UserViewModels
{
    public class EnterSMSCodeViewModel
    {
        public bool ModelStateValid { get; set; } = true;
        public string PhoneLast { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
