using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.CourseViewModels
{
    public class CreateViewModel : CommonViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public double Price { get; set; }
        public bool DisplayOwnerInfo { get; set; }
    }
}
