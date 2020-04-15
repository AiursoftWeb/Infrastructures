using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.SDK.Models.API.UserAddressModels
{
    public class DropGrantedAppsAddressModel : UserOperationAddressModel
    {
        [Required]
        public string AppIdToDrop { get; set; }
    }
}
