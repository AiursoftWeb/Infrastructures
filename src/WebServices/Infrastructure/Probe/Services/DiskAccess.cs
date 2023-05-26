using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Probe.Models.Configuration;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.Services;

public class DiskAccess : IStorageProvider
{
    private readonly char _ = Path.DirectorySeparatorChar;
    private readonly AiurCache _cache;
    private readonly string _path;
    private readonly string _trashPath;

    public DiskAccess(
        IOptions<DiskAccessConfig> config,
        AiurCache aiurCache)
    {
        _path = config.Value.StoragePath + $"{_}Storage{_}";
        var tempFilePath = config.Value.StoragePath;
        if (string.IsNullOrWhiteSpace(tempFilePath))
        {
            tempFilePath = config.Value.StoragePath;
        }

        _trashPath = tempFilePath + $"{_}TrashBin{_}";
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
        if (!File.Exists(path))
        {
            return;
        }

        if (!System.IO.Directory.Exists(_trashPath))
        {
            System.IO.Directory.CreateDirectory(_trashPath);
        }

        File.Move(path, target);
    }

    public bool ExistInHardware(string hardwareUuid)
    {
        var path = _path + $"{hardwareUuid}.dat";
        return File.Exists(path);
    }

    public string[] GetAllFileNamesInHardware()
    {
        if (!System.IO.Directory.Exists(_path))
        {
            System.IO.Directory.CreateDirectory(_path);
        }

        return System.IO.Directory.GetFiles(_path).Select(t => Path.GetFileNameWithoutExtension(t)).ToArray();
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
        if (!System.IO.Directory.Exists(_path))
        {
            System.IO.Directory.CreateDirectory(_path);
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

        return 0;
    }
}