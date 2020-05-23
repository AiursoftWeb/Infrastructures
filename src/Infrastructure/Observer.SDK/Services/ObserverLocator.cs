namespace Aiursoft.Observer.SDK.Services
{
    public class ObserverLocator
    {
        public ObserverLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
