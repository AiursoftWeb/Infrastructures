using System.ComponentModel.DataAnnotations;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class EditDesViewModel : ProfileViewModelBase
    {
        [Required]
        [Display(Name = "Edit introduction about you")]
        [MinLength(50)]
        [MaxLength(500)]
        public string NewDes { get; set; }
    }
}
