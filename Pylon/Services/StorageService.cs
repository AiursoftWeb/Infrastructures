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
        private readonly TokenService _tokenService;

        public StorageService(
            AppsContainer appsContainer,
            FilesService filesService,
            TokenService tokenService)
        {
            _appsContainer = appsContainer;
            _filesService = filesService;
            _tokenService = tokenService;
        }

        [Obsolete(message: "We strongly suggest using the PBToken + Probe API to upload file to Probe to get better performance.")]
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
            var pbToken = await _tokenService.GetUploadTokenAsync(accessToken, siteName, "Upload", path);
            var result = await _filesService.UploadFileAsync(pbToken, siteName, path, file.OpenReadStream(), fileName, true);
            return result;
        }

        public static string GetProbeDownloadAddress(ServiceLocation serviceLocation, string siteName, string path, string fileName)
        {
            var filePath = $"{siteName}/{path}/{fileName}".TrimStart('/');
            return GetProbeDownloadAddress(serviceLocation, filePath);
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
