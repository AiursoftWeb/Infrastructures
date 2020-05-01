using Aiursoft.SDK.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class DiskAccess : IStorageProvider
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly IConfiguration _configuration;
        private readonly AiurCache _cache;

        public DiskAccess(
            IConfiguration configuration,
            AiurCache aiurCache)
        {
            _configuration = configuration;
            _cache = aiurCache;
        }

        public void Delete(string hardwareUuid)
        {
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{hardwareUuid}.dat";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public bool ExistInHardware(string hardwareUuid)
        {
            var path = _configuration["StoragePath"] + $"{_}Storage{_}{hardwareUuid}.dat";
            return File.Exists(path);
        }

        public string[] GetAllFileNamesInHardware()
        {
            return Directory.GetFiles(_configuration["StoragePath"] + $"{_}Storage");
        }

        public string GetExtension(string fileName)
        {
            return Path.GetExtension(fileName).TrimStart('.').ToLower();
        }

        public string GetFilePath(string hardwareUuid)
        {
            var path = _configuration["StoragePath"] + $"{_}Storage{_}{hardwareUuid}.dat";
            return path;
        }

        public long GetSize(string hardwareUuid)
        {
            return _cache.GetAndCache($"file-size-cache-id-{hardwareUuid}", () => GetFileSize(hardwareUuid));
        }

        public async Task Save(string hardwareUuid, IFormFile file)
        {
            //Try saving file.
            var directoryPath = _configuration["StoragePath"] + $"{_}Storage{_}";
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
            using var fileStream = new FileStream(directoryPath + hardwareUuid + ".dat", FileMode.Create);
            await file.CopyToAsync(fileStream);
            fileStream.Close();
        }

        private long GetFileSize(string hardwareUuid)
        {
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{hardwareUuid}.dat";
            if (File.Exists(path))
            {
                return new FileInfo(path).Length;
            }
            else
            {
                return 0;
            }
        }
    }
}
