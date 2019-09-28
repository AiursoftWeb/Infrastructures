using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class EnableAuthenticatorViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public EnableAuthenticatorViewModel() { }
        public EnableAuthenticatorViewModel(AccountUser user) : base(user, 6, "Authentication")
        {
            Recover(user);
        }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 6, "Authentication");
            TwoFASharedKey = user.TwoFASharedKey;
            //Code = user.Code;
            //SharedKey = user.SharedKey;
            //AuthenticatorUri = user.AuthenticatorUri;
        }

        //[Required]
        //[StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Text)]
        //[Display(Name = "Verification Code")]
        public string TwoFACode { get; set; }

       // [BindNever]
        public virtual string TwoFASharedKey { get; set; }

        [BindNever]
        public string TowFAuthenticatorUri { get; set; }

    }
}
