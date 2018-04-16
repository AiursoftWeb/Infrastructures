using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.Stargate.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocal
    {
        public string AppId { get; set; }
        public IEnumerable<Channel> Channel { get; set; }
    }
}
