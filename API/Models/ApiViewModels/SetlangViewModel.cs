using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models.ApiViewModels
{
    public class SetlangViewModel : SetlangAddressModel
    {
        [Required]
        public string Culture { get; set; }
    }
    public class SetlangAddressModel
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
