using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Observer.SDK.Models.EventAddressModels;

public class ViewAddressModel
{
    [Required] public string AccessToken { get; set; }
}