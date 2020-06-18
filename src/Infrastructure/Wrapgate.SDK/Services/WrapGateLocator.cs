namespace Aiursoft.Wrapgate.SDK.Services
{
    public class WrapgateLocator
    {
        public WrapgateLocator(string endpoint, string wrapPattern)
        {
            Endpoint = endpoint;
            WrapPattern = wrapPattern;
        }

        public string Endpoint { get; }
        public string WrapPattern { get; }
    }
}
