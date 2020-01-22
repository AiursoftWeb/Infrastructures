using Aiursoft.XelNaga.Models; using Aiursoft.Handler.Models;

namespace Aiursoft.SDK.Models.Stargate.ChannelViewModels
{
    public class CreateChannelViewModel : AiurProtocol
    {
        public int ChannelId { get; set; }
        public string ConnectKey { get; set; }
    }
}
