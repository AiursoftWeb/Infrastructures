using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Archon.SDK.Models
{
    public class AccessTokenAddressModel
    {
        [Required]
        public virtual string AppId { get; set; }
        [Required]
        public virtual string AppSecret { get; set; }
    }
}
