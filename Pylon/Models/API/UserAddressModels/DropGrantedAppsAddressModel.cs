using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class DropGrantedAppsAddressModel : UserOperationAddressModel
    {
        public string AppIdToDrop { get; set; }
    }
}
