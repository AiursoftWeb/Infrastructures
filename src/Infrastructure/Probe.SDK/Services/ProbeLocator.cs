using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.XelNaga.Tools;
using System;
using System.Linq;

namespace Aiursoft.Probe.SDK.Services
{
    public class ProbeLocator
    {
        public ProbeLocator(
            string endpoint,
            string openFormat,
            string downloadFormat)
        {
            Endpoint = endpoint;
            if (string.IsNullOrWhiteSpace(openFormat))
            {
                openFormat = endpoint + "/download/open/{0}";
            }
            OpenFormat = openFormat;
            if (string.IsNullOrWhiteSpace(DownloadFormat))
            {
                downloadFormat = endpoint + "/download/file/{0}";
            }
            DownloadFormat = downloadFormat;
        }

        public string Endpoint { get; private set; }
        public string OpenFormat { get; private set; }
        public string DownloadFormat { get; private set; }

        public string GetProbeOpenAddress(string siteName, string path, string fileName)
        {
            var fullPath = GetProbeFullPath(siteName, path, fileName);
            return GetProbeOpenAddress(fullPath);
        }

        public string GetProbeDownloadAddress(string siteName, string path, string fileName)
        {
            var fullPath = GetProbeFullPath(siteName, path, fileName);
            return GetProbeDownloadAddress(fullPath);
        }

        public string GetProbeFullPath(string siteName, string path, string fileName)
        {
            var filePath = $"{path}/{fileName}".TrimStart('/');
            var fullPath = $"{siteName}/{filePath}".TrimStart('/');
            return fullPath;
        }

        public string GetProbeOpenAddress(string fullpath)
        {
            var (siteName, folders, fileName) = SplitToPath(fullpath);
            var domain = string.Format(OpenFormat, siteName);
            var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
            return $"{domain}/{path}{fileName.ToUrlEncoded()}";
        }

        public string GetProbeDownloadAddress(string fullpath)
        {
            var (siteName, folders, fileName) = SplitToPath(fullpath);
            var domain = string.Format(DownloadFormat, siteName);
            var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
            return $"{domain}/{path}{fileName.ToUrlEncoded()}";
        }

        private (string siteName, string[] folders, string fileName) SplitToPath(string fullpath)
        {
            if (fullpath == null || fullpath.Length == 0)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, $"Can't get your file download address from path: '{fullpath}'!");
            }
            var paths = SplitStrings(fullpath);
            var fileName = paths.Last();
            var siteName = paths.First();
            var folders = paths.Take(paths.Count() - 1).Skip(1).ToArray();
            return (siteName, folders, fileName);
        }

        private string[] SplitStrings(string folderNames)
        {
            return folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }
    }
}
