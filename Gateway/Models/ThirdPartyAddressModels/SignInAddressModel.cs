using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Models.ThirdPartyAddressModels
{
    public class SignInAddressModel
    {
        public string ProviderName { get; set; }
        public string Code { get; set; }
        public string State { get; set; }
    }
}
