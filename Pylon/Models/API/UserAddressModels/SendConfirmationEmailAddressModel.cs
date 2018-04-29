using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class SendConfirmationEmailAddressModel : WithAccessTokenAddressModel
    {
        public string Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
