using Microsoft.AspNetCore.Mvc;
using System;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class NewFileViewModel : LayoutViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public NewFileViewModel() { }
        public NewFileViewModel(ColossusUser user) : base(user, 2, "Upload new file") { }
        public void Recover(ColossusUser user)
        {
            RootRecover(user, 5, "Upload new file");
        }
        [FromRoute]
        public string Path { get; set; }
    }
}
