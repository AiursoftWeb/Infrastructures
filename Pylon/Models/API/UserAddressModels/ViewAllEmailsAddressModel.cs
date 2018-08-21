using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class SetPhoneNumberAddressModel : UserOperationAddressModel
    {
        public string Phone { get; set; }
    }
}
