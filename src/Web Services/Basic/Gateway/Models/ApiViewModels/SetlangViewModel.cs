using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.ApiViewModels
{
    public class SetLangViewModel : SetLangAddressModel
    {
        [Required]
        public string Culture { get; set; }
    }
    public class SetLangAddressModel
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
