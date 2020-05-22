namespace Aiursoft.Observer.SDK.Services
{
    public class StatusLocator
    {
        public StatusLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
