using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Models.ThirdyPartyViewModels
{
    public class SignInViewModel
    {
        public OAuthInfo OAuthInfo { get; set; }
        public IUserDetail UserDetail { get; set; }
        public string ProviderName { get; set; }
        public string AppImageUrl { get; set; }
        public bool CanFindAnAccountWithEmail { get; set; }
        public IAuthProvider Provider { get; internal set; }

        // To Submit
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Aiursoft Account")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [NoSpace]
        public string Password { get; set; }
    }
}
