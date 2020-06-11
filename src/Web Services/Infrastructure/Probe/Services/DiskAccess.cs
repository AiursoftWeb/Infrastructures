using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class DiskAccess : IStorageProvider
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly string _path;
        private readonly string _trashPath;
        private readonly AiurCache _cache;

        public DiskAccess(
            IConfiguration configuration,
            AiurCache aiurCache)
        {
            _path = configuration["StoragePath"] + $"{_}Storage{_}";
            var tempFilePath = configuration["TempFileStoragePath"];
            if (string.IsNullOrWhiteSpace(tempFilePath))
            {
                tempFilePath = configuration["StoragePath"];
            }
            _trashPath = tempFilePath + $"{_}trashbin{_}";
            _cache = aiurCache;
        }

        public void Delete(string hardwareUuid)
        {
            var path = _path + $"{hardwareUuid}.dat";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteToTrash(string hardwareUuid)
        {
            var path = _path + $"{hardwareUuid}.dat";
            var target = _trashPath + $"{hardwareUuid}.dat";
            if (File.Exists(path))
            {
                if (Directory.Exists(_trashPath) == false)
                {
                    Directory.CreateDirectory(_trashPath);
                }
                File.Move(path, target);
            }
        }

        public bool ExistInHardware(string hardwareUuid)
        {
            var path = _path + $"{hardwareUuid}.dat";
            return File.Exists(path);
        }

        public string[] GetAllFileNamesInHardware()
        {
            return Directory.GetFiles(_path);
        }

        public string GetExtension(string fileName)
        {
            return Path.GetExtension(fileName).TrimStart('.').ToLower();
        }

        public string GetFilePath(string hardwareUuid)
        {
            var path = _path + $"{hardwareUuid}.dat";
            return path;
        }

        public long GetSize(string hardwareUuid)
        {
            return _cache.GetAndCache($"file-size-cache-id-{hardwareUuid}", () => GetFileSize(hardwareUuid));
        }

        public async Task Save(string hardwareUuid, IFormFile file)
        {
            //Try saving file.
            if (Directory.Exists(_path) == false)
            {
                Directory.CreateDirectory(_path);
            }
            using var fileStream = new FileStream(_path + hardwareUuid + ".dat", FileMode.Create);
            await file.CopyToAsync(fileStream);
            fileStream.Close();
        }

        private long GetFileSize(string hardwareUuid)
        {
            var path = _path + $"{hardwareUuid}.dat";
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
