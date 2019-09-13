using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class DeleteFileViewModel : LayoutViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteFileViewModel() { }
        public DeleteFileViewModel(ColossusUser user) : base(user, 1, "Delete file")
        {

        }

        public void Recover(ColossusUser user)
        {
            RootRecover(user, 1, "Delete file");
        }

        [Required]
        public string Path { get; set; }
    }
}
