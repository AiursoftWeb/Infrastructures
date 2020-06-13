using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Wrapgate.SDK.Models.AddressModels
{
    public class WrapAddressModel
    {
        [FromRoute]
        [ValidDomainName]
        public string RecordName { get; set; }
        [FromRoute]
        public string Path { get; set; }
    }
}
