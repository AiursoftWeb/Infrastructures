using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class DropGrantedAppsAddressModel : UserOperationAddressModel
    {
        [Required]
        public string AppIdToDrop { get; set; }
    }
}
