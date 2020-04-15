namespace Aiursoft.Gateway.SDK.Services
{
    public class GatewayLocator
    {
        public GatewayLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
