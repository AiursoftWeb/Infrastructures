﻿using Aiursoft.SDK.Services;
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

        public void Delete(int id)
        {
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{id}.dat";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GetExtension(string fileName)
        {
            return Path.GetExtension(fileName).TrimStart('.').ToLower();
        }

        public string GetFilePath(int fileId)
        {
            var path = _configuration["StoragePath"] + $"{_}Storage{_}{fileId}.dat";
            return path;
        }

        public long GetSize(int id)
        {
            return _cache.GetAndCache($"file-size-cache-id-{id}", () => GetFileSize(id));
        }

        public async Task Save(int id, IFormFile file)
        {
            //Try saving file.
            var directoryPath = _configuration["StoragePath"] + $"{_}Storage{_}";
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (var fileStream = new FileStream(directoryPath + id + ".dat", FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }
        }

        private long GetFileSize(int id)
        {
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{id}.dat";
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
