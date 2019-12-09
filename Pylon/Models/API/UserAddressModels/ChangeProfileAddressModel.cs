using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class ChangeProfileAddressModel : UserOperationAddressModel
    {
        [Required]
        [MaxLength(20)]
        public string NewNickName { get; set; }
        [Required]
        public string NewIconFilePathName { get; set; }
        [MaxLength(80)]
        public string NewBio { get; set; }
    }
}
