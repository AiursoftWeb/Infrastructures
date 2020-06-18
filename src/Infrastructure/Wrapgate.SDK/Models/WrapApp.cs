using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Wrapgate.SDK.Models
{
    public class WrapgateApp
    {
        [Key]
        public string AppId { get; set; }

        [InverseProperty(nameof(WrapRecord.App))]
        public IEnumerable<WrapRecord> WrapRecords { get; set; }
    }
}
