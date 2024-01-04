using Aiursoft.AiurObserver;

namespace Aiursoft.Stargate.SDK.Models;

public class Channel
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string ConnectKey { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public DateTime LastAccessTime { get; set; } = DateTime.UtcNow;
    public string AppId { get; set; }
    
    public AsyncObservable<string> Messages { get; set; } = new();
}