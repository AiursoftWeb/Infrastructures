namespace Aiursoft.Warpgate.SDK.Services;

public class WarpgateLocator
{
    public WarpgateLocator(string endpoint, string warpPattern)
    {
        Endpoint = endpoint;
        WarpPattern = warpPattern;
    }

    public string Endpoint { get; }
    public string WarpPattern { get; }
}