using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.ForApps.AddressModels
{
    public class AuthResultAddressModel
    {
        public string State { get; set; }
        [Required]
        public int Code { get; set; }
    }
}
