using Aiursoft.SDKTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Wrapgate.SDK.Models.AddressModels
{
    public class UpdateRecordInfoAddressModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        [ValidDomainName]
        public string OldRecordName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        public string NewRecordName { get; set; }
        [Required]
        public RecordType NewType { get; set; }
        [Required]
        [MaxLength(1000)]
        [MinLength(5)]
        [Url]
        public string NewUrl { get; set; }

        [Required]
        public bool Enabled { get; set; }

        public string Tags { get; set; }
    }
}
