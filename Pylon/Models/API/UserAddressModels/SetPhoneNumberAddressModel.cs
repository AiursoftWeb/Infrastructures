using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class SetPhoneNumberAddressModel : WithAccessTokenAddressModel
    {
        [Required]
        public string OpenId { get; set; }
        public string Phone { get; set; }
    }
    public class ViewPhoneNumberAddressModel : WithAccessTokenAddressModel
    {
        [Required]
        public string OpenId { get; set; }
    }
    public class ViewAllEmailsAddressModel : WithAccessTokenAddressModel
    {
        [Required]
        public string OpenId { get; set; }
    }
}
