using System;

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

        public int MaxSize { get; set; }
    }
}
