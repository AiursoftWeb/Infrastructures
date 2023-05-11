using Aiursoft.Handler.Models;

namespace Aiursoft.Stargate.SDK.Models.HomeViewModels;

public class IndexViewModel : AiurProtocol
{
    public int CurrentId { get; set; }
    public int TotalMemoryMessages { get; set; }
    public int Channels { get; set; }
}