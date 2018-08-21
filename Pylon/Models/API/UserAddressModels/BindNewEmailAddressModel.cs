using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class BindNewEmailAddressModel : UserOperationAddressModel
    {
        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
