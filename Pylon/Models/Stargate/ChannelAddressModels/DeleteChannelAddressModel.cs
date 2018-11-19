using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Stargate.ChannelAddressModels
{
    public class DeleteChannelAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int ChannelId { get; set; }
    }
}
