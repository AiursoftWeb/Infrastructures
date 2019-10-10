using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class SetTwoFAAddressModel : UserOperationAddressModel
    {
        public string Code { get; set; }
        public string RecoveryCodesKey { get; set; }

        public string AuthenticatorTokenProvider { get; set; }
    }
}
