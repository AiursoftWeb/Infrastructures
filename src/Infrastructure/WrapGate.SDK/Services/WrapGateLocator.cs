namespace Aiursoft.Wrapgate.SDK.Services
{
    public class WrapgateLocator
    {
        public WrapgateLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
