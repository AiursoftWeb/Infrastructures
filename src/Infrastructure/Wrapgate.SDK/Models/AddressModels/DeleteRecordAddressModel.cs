using Aiursoft.SDKTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Wrapgate.SDK.Models.AddressModels
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
