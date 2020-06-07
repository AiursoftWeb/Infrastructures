using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Wrapgate.SDK.Models.AddressModels
{
    public class DeleteRecordAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RecordName { get; set; }
    }
}
