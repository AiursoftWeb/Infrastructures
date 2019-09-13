using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class DeleteFolderViewModel : LayoutViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteFolderViewModel() { }
        public DeleteFolderViewModel(ColossusUser user) : base(user, 2, "Delete folder")
        {

        }

        public void Recover(ColossusUser user)
        {
            RootRecover(user, 5, "Delete file");
        }

        [Required]
        public string Path { get; set; }
    }
}
