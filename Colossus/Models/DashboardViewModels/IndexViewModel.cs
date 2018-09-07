using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class IndexViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public IndexViewModel()
        {
        }

        public IndexViewModel(ColossusUser user, int activePanel, string title) : base(user, activePanel, title)
        {
        }

        public IEnumerable<UploadRecord> Files { get; set; }
    }
}
