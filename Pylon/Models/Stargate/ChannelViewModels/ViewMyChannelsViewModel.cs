using System.Collections.Generic;

namespace Aiursoft.Pylon.Models.Stargate.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public IEnumerable<Channel> Channel { get; set; }
    }
}
