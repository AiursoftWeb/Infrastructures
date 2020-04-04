namespace Aiursoft.Archon.SDK.Services
{
    public class ArchonLocator
    {
        public ArchonLocator(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
