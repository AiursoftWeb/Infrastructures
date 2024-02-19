using Aiursoft.AiurProtocol.Models;

namespace Aiursoft.Stargate.SDK.Models.ChannelViewModels;

public class CreateChannelViewModel : AiurResponse
{
    public int ChannelId { get; set; }
    public string ConnectKey { get; set; }
}