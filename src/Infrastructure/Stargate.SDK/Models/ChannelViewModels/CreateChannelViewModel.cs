using Aiursoft.Handler.Models;

namespace Aiursoft.Stargate.SDK.Models.ChannelViewModels;

public class CreateChannelViewModel : AiurProtocol
{
    public int ChannelId { get; set; }
    public string ConnectKey { get; set; }
}