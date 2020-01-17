using Aiursoft.XelNaga.Models;
using System.Collections.Generic;

namespace Aiursoft.SDK.Models.Stargate.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public IEnumerable<ChannelDetail> Channels { get; set; }
    }
}
