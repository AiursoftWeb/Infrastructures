using Aiursoft.SDKTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.SDK.Models.ApiAddressModels
{
    public class IsValidateAppAddressModel
    {
        [Required]
        [IsGuidOrEmpty]
        public virtual string? AppId { get; set; }
        [Required]
        public virtual string? AppSecret { get; set; }
    }
}
