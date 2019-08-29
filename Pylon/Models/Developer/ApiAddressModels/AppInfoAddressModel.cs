using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Developer.ApiAddressModels
{
    public class AppInfoAddressModel
    {
        [Required]
        public virtual string AppId { get; set; }
    }
}
