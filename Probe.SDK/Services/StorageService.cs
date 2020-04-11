using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.XelNaga.Tools;
using System;
using System.Linq;

namespace Aiursoft.Probe.SDK.Services
{
    public static class StorageService
    {
        public static string GetProbeOpenAddress(this ProbeLocator probeLocator, string siteName, string path, string fileName)
        {
            var fullPath = GetProbeFullPath(siteName, path, fileName);
            return GetProbeOpenAddress(probeLocator, fullPath);
        }

        public static string GetProbeDownloadAddress(this ProbeLocator probeLocator, string siteName, string path, string fileName)
        {
            var fullPath = GetProbeFullPath(siteName, path, fileName);
            return GetProbeDownloadAddress(probeLocator, fullPath);
        }

        public static string GetProbeFullPath(string siteName, string path, string fileName)
        {
            var filePath = $"{path}/{fileName}".TrimStart('/');
            var fullPath = $"{siteName}/{filePath}".TrimStart('/');
            return fullPath;
        }

        public static string GetProbeOpenAddress(ProbeLocator probeLocator, string fullpath)
        {
            var (siteName, folders, fileName) = SplitToPath(fullpath);
            var domain = string.Format(probeLocator.ProbeOpenCDN, siteName);
            var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
            return $"{domain}/{path}{fileName.ToUrlEncoded()}";
        }

        public static string GetProbeDownloadAddress(ProbeLocator probeLocator, string fullpath)
        {
            var (siteName, folders, fileName) = SplitToPath(fullpath);
            var domain = string.Format(probeLocator.ProbeDownloadCDN, siteName);
            var path = (string.Join('/', folders).EncodePath() + "/").TrimStart('/');
            return $"{domain}/{path}{fileName.ToUrlEncoded()}";
        }

        private static (string siteName, string[] folders, string fileName) SplitToPath(string fullpath)
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

        private static string[] SplitStrings(string folderNames)
        {
            return folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }
    }

    public enum SaveFileOptions
    {
        RandomName,
        SourceName
    }
}
