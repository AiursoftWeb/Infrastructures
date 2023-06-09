using System;
using System.IO;
using System.Threading.Tasks;
using Aiursoft.Probe.Models.Configuration;
using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Aiursoft.Probe.Services;

public class ImageCompressor : ITransientDependency
{
    private static readonly object ObjCompareLock = new();
    private static readonly object ObjClearLock = new();
    private readonly ILogger<ImageCompressor> _logger;
    private readonly SizeCalculator _sizeCalculator;
    private readonly string _tempFilePath;

    public ImageCompressor(
        ILogger<ImageCompressor> logger,
        IOptions<DiskAccessConfig> diskAccessConfig,
        SizeCalculator sizeCalculator)
    {
        _logger = logger;
        _sizeCalculator = sizeCalculator;
        _tempFilePath = diskAccessConfig.Value.TempFileStoragePath;
        if (string.IsNullOrWhiteSpace(_tempFilePath))
        {
            _tempFilePath = diskAccessConfig.Value.StoragePath;
        }
    }

    public async Task<string> ClearExif(string path)
    {
        try
        {
            var clearedFolder =
                _tempFilePath + $"{Path.DirectorySeparatorChar}ClearedEXIF{Path.DirectorySeparatorChar}";
            if (System.IO.Directory.Exists(clearedFolder) == false)
            {
                System.IO.Directory.CreateDirectory(clearedFolder);
            }

            var clearedImagePath = $"{clearedFolder}probe_cleared_file_id_{Path.GetFileNameWithoutExtension(path)}.dat";
            await ClearImage(path, clearedImagePath);
            return clearedImagePath;
        }
        catch (ImageFormatException ex)
        {
            _logger.LogError(ex, "Failed to clear the EXIF of an image");
            return path;
        }
    }

    private async Task ClearImage(string sourceImage, string saveTarget)
    {
        var fileExists = File.Exists(saveTarget);
        var fileCanRead = FileCanBeRead(saveTarget);
        if (fileExists && fileCanRead)
            // Return. Because we have already cleared it.
        {
            return;
        }

        if (fileExists)
        {
            while (!FileCanBeRead(saveTarget))
            {
                await Task.Delay(500);
            }
        }
        else
        {
            lock (ObjClearLock)
            {
                var image = Image.Load(sourceImage);
                image.Mutate(x => x.AutoOrient());
                image.Metadata.ExifProfile = null;
                image.Save(saveTarget ?? throw new NullReferenceException($"When compressing image, {nameof(saveTarget)} is null!"));
            }
        }
    }

    public async Task<string> Compress(string path, int width, int height)
    {
        width = _sizeCalculator.Ceiling(width);
        height = _sizeCalculator.Ceiling(height);
        try
        {
            var compressedFolder =
                _tempFilePath + $"{Path.DirectorySeparatorChar}Compressed{Path.DirectorySeparatorChar}";
            if (System.IO.Directory.Exists(compressedFolder) == false)
            {
                System.IO.Directory.CreateDirectory(compressedFolder);
            }

            var compressedImagePath =
                $"{compressedFolder}probe_compressed_w{width}_h{height}_fileId_{Path.GetFileNameWithoutExtension(path)}.dat";
            await SaveCompressedImage(path, compressedImagePath, width, height);
            return compressedImagePath;
        }
        catch (ImageFormatException ex)
        {
            _logger.LogError(ex, "Failed to compress an image");
            return path;
        }
    }

    private async Task SaveCompressedImage(string sourceImage, string saveTarget, int width, int height)
    {
        var fileExists = File.Exists(saveTarget);
        var fileCanRead = FileCanBeRead(saveTarget);
        if (fileExists && fileCanRead)
            // Return. Because we have already cleared it.
        {
            return;
        }

        if (fileExists)
        {
            while (!FileCanBeRead(saveTarget))
            {
                await Task.Delay(500);
            }
        }
        else
        {
            lock (ObjCompareLock)
            {
                var image = Image.Load(sourceImage);
                image.Mutate(x => x.AutoOrient());
                image.Metadata.ExifProfile = null;
                image.Mutate(x => x
                    .Resize(width, height));
                image.Save(saveTarget ?? throw new NullReferenceException($"When compressing image, {nameof(saveTarget)} is null!"));
            }
        }
    }

    private bool FileCanBeRead(string filepath)
    {
        try
        {
            File.Open(filepath, FileMode.Open, FileAccess.Read).Dispose();
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}