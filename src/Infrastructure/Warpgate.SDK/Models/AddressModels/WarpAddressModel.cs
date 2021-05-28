using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels
{
    public class WarpAddressModel
    {
        [FromRoute]
        [ValidDomainName]
        public string RecordName { get; set; }
        [FromRoute]
        public string Path { get; set; }
    }
}
