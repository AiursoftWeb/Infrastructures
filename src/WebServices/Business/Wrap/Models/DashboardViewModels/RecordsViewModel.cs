using Aiursoft.Wrapgate.SDK.Models;
using Aiursoft.Wrapgate.SDK.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Models.DashboardViewModels
{
    public class RecordsViewModel : LayoutViewModel
    {
        public RecordsViewModel(WrapUser user, List<WrapRecord> records) : base(user, "My records")
        {
            Records = records;
        }

        public List<WrapRecord> Records { get; }
    }
}
