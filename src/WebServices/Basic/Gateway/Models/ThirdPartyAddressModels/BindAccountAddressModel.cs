using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Gateway.Models.ThirdPartyAddressModels
{
    public class BindAccountAddressModel
    {
        [FromRoute]
        public string ProviderName { get; set; }
        [FromQuery(Name = "code")]
        public string Code { get; set; }
    }
}
