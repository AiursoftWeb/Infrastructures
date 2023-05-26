using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.SDK.Models.API.UserAddressModels;

public class DropGrantedAppsAddressModel : UserOperationAddressModel
{
    [Required] public string AppIdToDrop { get; set; }
}