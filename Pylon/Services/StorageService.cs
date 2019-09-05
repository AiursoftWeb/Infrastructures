using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.Probe.FilesViewModels;
using Aiursoft.Pylon.Services.ToProbeServer;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using static System.IO.Path;

namespace Aiursoft.Pylon.Services
{
    public class StorageService
    {
        private readonly AppsContainer _appsContainer;
        private readonly FilesService _filesService;

        public StorageService(
            AppsContainer appsContainer,
            FilesService filesService)
        {
            _appsContainer = appsContainer;
            _filesService = filesService;
        }

        public async Task<UploadFileViewModel> SaveToProbe(IFormFile file, string siteName, string path, SaveFileOptions options, string accessToken = null)
        {
            string fileName = options == SaveFileOptions.RandomName ?
                Guid.NewGuid().ToString("N") + GetExtension(file.FileName) :
                file.FileName.Replace(" ", "_");
            if (!new ValidFolderName().IsValid(fileName))
            {
                throw new InvalidOperationException($"The file with name: '{fileName}' is invalid!");
            }
            if (accessToken == null)
            {
                accessToken = await _appsContainer.AccessToken();
            }
            var result = await _filesService.UploadFileAsync(accessToken, siteName, path, file.OpenReadStream(), fileName, true);
            return result;
        }

        public static string GetProbeDownloadAddress(ServiceLocation serviceLocation, string siteName, string path, string fileName)
        {
            var filePath = $"{path}/{fileName}".TrimStart('/');
            return $"{serviceLocation.Probe}/Download/Open/{siteName.ToUrlEncoded()}/{filePath.EncodePath()}";
        }

        /// <summary>
        /// Get downloadable file address for probe content.
        /// </summary>
        /// <param name="serviceLocation"></param>
        /// <param name="fullpath">sitename/filepath/filename.extension</param>
        /// <returns></returns>
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
