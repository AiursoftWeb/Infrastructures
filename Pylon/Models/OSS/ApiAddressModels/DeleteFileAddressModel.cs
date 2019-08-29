using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.OSS.ApiAddressModels
{
    public class DeleteFileAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        public int FileKey { get; set; }
    }
}