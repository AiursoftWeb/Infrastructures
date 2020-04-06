namespace Aiursoft.Stargate.SDK.Services
{
    public class StargateLocator
    {
        public StargateLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
