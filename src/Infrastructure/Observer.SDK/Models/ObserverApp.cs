using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Observer.SDK.Models
{
    public class ObserverApp
    {
        [Key]
        public string AppId { get; set; }

        [InverseProperty(nameof(ErrorLog.Context))]
        public IEnumerable<ErrorLog> ErrorLogs { get; set; }
    }
}
