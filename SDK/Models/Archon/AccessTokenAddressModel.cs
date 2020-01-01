using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Archon
{
    public class AccessTokenAddressModel
    {
        [Required]
        public virtual string AppId { get; set; }
        [Required]
        public virtual string AppSecret { get; set; }
    }
}
