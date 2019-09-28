using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    class SetTwoFAAddressModel: UserOperationAddressModel
    {
        //[Required]
        //[MaxLength(20)]
       // public string TwoFACode { get; set; }
        //[Required]
        public string TwoFASharedKey { get; set; }
        //[MaxLength(80)]
       // public string TowFAuthenticatorUri { get; set; }
    }
}
