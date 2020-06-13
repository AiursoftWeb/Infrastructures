using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Wrapgate.SDK.Models.AddressModels
{
    public class ViewMyRecordsAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
