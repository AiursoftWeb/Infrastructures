using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Wrapgate.SDK.Models
{
    public class WrapRecord
    {
        [Key]
        public int Id { get; set; }
        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        public WrapgateApp App { get; set; }
    }
}
