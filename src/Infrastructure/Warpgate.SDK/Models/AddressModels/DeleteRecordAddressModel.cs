using Aiursoft.SDKTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels
{
    public class DeleteRecordAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        [ValidDomainName]
        public string RecordName { get; set; }
    }
}
