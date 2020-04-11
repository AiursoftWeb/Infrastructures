namespace Aiursoft.Probe.SDK.Services
{
    public class ProbeLocator
    {
        public ProbeLocator(
            string endpoint,
            string openCDN,
            string downloadCDN,
            string probeIO)
        {
            Endpoint = endpoint;
            ProbeOpenCDN = openCDN;
            ProbeDownloadCDN = downloadCDN;
            ProbeIO = probeIO;
        }

        public string Endpoint { get; private set; }
        public string ProbeOpenCDN { get; private set; }
        public string ProbeDownloadCDN { get; private set; }
        public string ProbeIO { get; private set; }
    }
}
