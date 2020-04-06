namespace Aiursoft.Stargate.SDK.Services
{
    public class StargateLocator
    {
        public StargateLocator(string endpoint)
        {
            Endpoint = endpoint;
            ListenEndpoint = Endpoint
            .Replace("https://", "wss://")
            .Replace("http://", "ws://");
        }

        public string Endpoint { get; private set; }
        public string ListenEndpoint { get; private set; }
    }
}
