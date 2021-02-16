using Aiursoft.Wrapgate.SDK.Models;
using System.Collections.Generic;

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
