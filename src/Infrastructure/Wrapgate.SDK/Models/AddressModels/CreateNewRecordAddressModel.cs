using Aiursoft.SDKTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Wrapgate.SDK.Models.AddressModels
{
    public class CreateNewRecordAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        [ValidDomainName]
        [MaxLength(50)]
        [MinLength(5)]
        public string NewRecordName { get; set; }
        [Required]
        public RecordType Type { get; set; }
        [Required]
        [MaxLength(1000)]
        [MinLength(5)]
        [Url]
        public string TargetUrl { get; set; }

        [Required]
        public bool Enabled { get; set; }
    }
}
