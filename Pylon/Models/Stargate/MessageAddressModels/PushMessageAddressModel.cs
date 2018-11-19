using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Stargate.MessageAddressModels
{
    public class PushMessageAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int ChannelId { get; set; }
        public string MessageContent { get; set; }
    }
}
