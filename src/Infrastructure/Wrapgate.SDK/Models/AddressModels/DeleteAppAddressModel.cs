using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Wrapgate.SDK.Models.AddressModels
{
    public class DeleteAppAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string AppId { get; set; }
    }
}
