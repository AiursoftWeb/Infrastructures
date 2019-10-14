using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class ViewTwoFAAddressModel : UserOperationAddressModel
    {
        public string TwoFAKey { get; set; }
        public bool HasAuthenticator { get; set; }
        public bool Is2faEnabled { get; set; }
    }
}
