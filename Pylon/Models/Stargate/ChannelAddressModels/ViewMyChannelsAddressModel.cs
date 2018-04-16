using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Stargate.ChannelAddressModels
{
    public class ViewMyChannelsAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
