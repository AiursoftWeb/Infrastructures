namespace Aiursoft.Probe.SDK.Services
{
    public class ProbeLocator
    {
        public ProbeLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
