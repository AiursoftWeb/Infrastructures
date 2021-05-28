using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels
{
    public class ViewMyRecordsAddressModel
    {
        [Required]
        public string AccessToken { get; set; }

        public string Tag { get; set; }
    }
}
