using Aiursoft.SDKTools.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
        public string TargetUrl { get; set; }
    }
}
