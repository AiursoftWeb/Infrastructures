using Aiursoft.Handler.Abstract.Models;
using System.Collections.Generic;

namespace Aiursoft.Stargate.SDK.Models.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public IEnumerable<ChannelDetail> Channels { get; set; }
    }
}
