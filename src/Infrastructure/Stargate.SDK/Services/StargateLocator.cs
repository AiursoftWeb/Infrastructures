namespace Aiursoft.Stargate.SDK.Services;

// TODO: Refactor this as configuration!
public class StargateLocator
{
    public StargateLocator(string endpoint)
    {
        Endpoint = endpoint;
        ListenEndpoint = Endpoint
            .Replace("https://", "wss://")
            .Replace("http://", "ws://");
    }

    public string Endpoint { get; }
    public string ListenEndpoint { get; private set; }
}