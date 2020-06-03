namespace Aiursoft.WrapGate.SDK.Services
{
    public class WrapGateLocator
    {
        public WrapGateLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
