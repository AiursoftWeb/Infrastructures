using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Developer.ApiAddressModels
{
    public class IsValidateAppAddressModel
    {
        [Required]
        public virtual string AppId { get; set; }
        [Required]
        public virtual string AppSecret { get; set; }
    }
}
