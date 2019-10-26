using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class Get2FAKeyAddressModel : UserOperationAddressModel
    {
        public string TwoFAKey { get; set; }
        public string TwoFAQRUri { get; set; }
    }
}
