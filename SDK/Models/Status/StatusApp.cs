using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.SDK.Models.Status
{
    public class StatusApp
    {
        [Key]
        public string AppId { get; set; }

        [InverseProperty(nameof(ErrorLog.Context))]
        public IEnumerable<ErrorLog> ErrorLogs { get; set; }
    }
}
