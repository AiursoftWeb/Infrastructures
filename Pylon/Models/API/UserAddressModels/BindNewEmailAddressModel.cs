using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class BindNewEmailAddressModel : WithAccessTokenAddressModel
    {
        [Required]
        public string OpenId { get; set; }
        [Required]
        [MaxLength(20)]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
