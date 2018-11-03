using Aiursoft.Pylon.Models.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class LogsViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public LogsViewModel()
        {
        }

        public LogsViewModel(ColossusUser user, int activePanel, string title) : base(user, activePanel, title)
        {
        }

        public IEnumerable<OSSFile> Files { get; set; }
        public IEnumerable<UploadRecord> Records { get; set; }
    }
}
