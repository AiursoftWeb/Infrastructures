using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.CourseViewModels
{
    public class DetailViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Subscribed { get; set; }
        public int Id { get; set; }
        public string AuthorName { get; set; }

        public bool IsOwner { get; set; }
        public bool DisplayOwnerInfo { get; set; }
    }
}
