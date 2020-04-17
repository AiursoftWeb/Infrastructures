using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class DeleteFileViewModel : LayoutViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteFileViewModel() { }
        public DeleteFileViewModel(ColossusUser user) : base(user, "Delete file")
        {

        }

        public void Recover(ColossusUser user)
        {
            RootRecover(user, "Delete file");
        }

        [Required]
        public string Path { get; set; }
    }
}
