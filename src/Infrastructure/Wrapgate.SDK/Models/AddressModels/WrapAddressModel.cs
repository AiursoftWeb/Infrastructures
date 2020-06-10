using Aiursoft.SDKTools.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

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
