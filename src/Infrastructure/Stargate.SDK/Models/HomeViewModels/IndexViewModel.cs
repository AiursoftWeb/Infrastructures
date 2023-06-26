using Aiursoft.AiurProtocol;

namespace Aiursoft.Stargate.SDK.Models.HomeViewModels;

public class IndexViewModel : AiurResponse
{
    public int CurrentId { get; set; }
    public int TotalMemoryMessages { get; set; }
    public int Channels { get; set; }
}