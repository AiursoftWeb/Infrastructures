using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
