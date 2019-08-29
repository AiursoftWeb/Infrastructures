using Aiursoft.Pylon.Models.Developer;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class DeleteAppViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public DeleteAppViewModel() { }
        public DeleteAppViewModel(DeveloperUser User) : base(User, 1)
        {

        }
        [Required]
        public virtual string AppId { get; set; }
        public virtual string AppName { get; set; }
    }
}
