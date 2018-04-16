using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class IndexViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public IndexViewModel() { }
        public IndexViewModel(AccountUser user) : base(user, 0, "Profile")
        {
            Recover(user);
        }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 0, "Profile");
            this.NickName = user.NickName;
            this.Bio = user.Bio;
        }
        [Required]
        [MaxLength(20)]
        [Display(Name = "Nick name")]
        public virtual string NickName { get; set; }
        [MaxLength(80)]
        public virtual string Bio { get; set; }
    }
}
