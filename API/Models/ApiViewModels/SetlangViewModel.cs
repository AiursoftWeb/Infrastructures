using System.ComponentModel.DataAnnotations;

namespace Aiursoft.API.Models.ApiViewModels
{
    public class SetlangViewModel : SetlangAddressModel
    {
        [Required]
        public string Culture { get; set; }
    }
    public class SetlangAddressModel
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
