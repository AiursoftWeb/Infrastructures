using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Wrapgate.SDK.Models
{
    public class WrapRecord
    {
        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        public WrapgateApp App { get; set; }
    }
}
