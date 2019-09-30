namespace Aiursoft.Pylon.Services
{
    public static class StorageService
    {
        public static string GetProbeDownloadAddress(this ServiceLocation serviceLocation, string siteName, string path, string fileName)
        {
            var fullPath = GetProbeFullPath(siteName, path, fileName);
            return GetProbeDownloadAddress(serviceLocation, fullPath);
        }

        public static string GetProbeFullPath(string siteName, string path, string fileName)
        {
            var filePath = $"{path}/{fileName}".TrimStart('/');
            var fullPath = $"{siteName}/{filePath}".TrimStart('/');
            return fullPath;
        }

        public static string GetProbeDownloadAddress(ServiceLocation serviceLocation, string fullpath)
        {
            return $"{serviceLocation.Probe}/Download/Open/{fullpath.EncodePath()}";
        }
    }

    public enum SaveFileOptions
    {
        RandomName,
        SourceName
    }
}
