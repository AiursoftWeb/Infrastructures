using Aiursoft.Scanner.Interfaces;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class ImageCompressor : ITransientDependency
    {
        private readonly IConfiguration _configuration;
        private readonly SizeCalculator _sizeCalculator;
        private readonly string _tempFilePath;
        private static object _objCompreLock;
        private static object _objClearLock;

        public ImageCompressor(
            IConfiguration configuration,
            SizeCalculator sizeCalculator)
        {
            _configuration = configuration;
            _sizeCalculator = sizeCalculator;
            var tempFilePath = _configuration["TempFileStoragePath"];
            if (string.IsNullOrWhiteSpace(tempFilePath))
            {
                tempFilePath = _configuration["StoragePath"];
            }
            _tempFilePath = tempFilePath;
        }

        public async Task<string> ClearExif(string path)
        {
            try
            {
                var clearedFolder = _tempFilePath + $"{Path.DirectorySeparatorChar}ClearedEXIF{Path.DirectorySeparatorChar}";
                if (Directory.Exists(clearedFolder) == false)
                {
                    Directory.CreateDirectory(clearedFolder);
                }
                var clearededImagePath = $"{clearedFolder}probe_cleared_file_id_{Path.GetFileNameWithoutExtension(path)}.dat";
                await ClearImage(path, clearededImagePath);
                return clearededImagePath;
            }
            catch (ImageFormatException)
            {
                return path;
            }
        }

        private async Task ClearImage(string sourceImage, string saveTarget)
        {
            var fileExists = File.Exists(saveTarget);
            var fileCanRead = FileCanBeRead(saveTarget);
            if (fileExists && fileCanRead)
            {
                return;
            }
            else if (fileExists && !fileCanRead)
            {
                while (!FileCanBeRead(saveTarget))
                {
                    await Task.Delay(500);
                }
            }
            else
            {
                lock (_objClearLock)
                {
                    using var stream = File.OpenWrite(saveTarget);
                    var image = Image.Load(sourceImage, out IImageFormat format);
                    image.Mutate(x => x.AutoOrient());
                    image.Metadata.ExifProfile = null;
                    image.Save(stream, format);
                    stream.Close();
                }
            }
        }

        public async Task<string> Compress(string path, int width, int height)
        {
            width = _sizeCalculator.Ceiling(width);
            height = _sizeCalculator.Ceiling(height);
            try
            {
                var compressedFolder = _tempFilePath + $"{Path.DirectorySeparatorChar}Compressed{Path.DirectorySeparatorChar}";
                if (Directory.Exists(compressedFolder) == false)
                {
                    Directory.CreateDirectory(compressedFolder);
                }
                var compressedImagePath = $"{compressedFolder}probe_compressed_w{width}_h{height}_fileId_{Path.GetFileNameWithoutExtension(path)}.dat";
                await SaveCompressedImage(path, compressedImagePath, width, height);
                return compressedImagePath;
            }
            catch (ImageFormatException)
            {
                return path;
            }
        }

        private async Task SaveCompressedImage(string sourceImage, string saveTarget, int width, int height)
        {
            var fileExists = File.Exists(saveTarget);
            var fileCanRead = FileCanBeRead(saveTarget);
            if (fileExists && fileCanRead)
            {
                return;
            }
            else if (fileExists && !fileCanRead)
            {
                while (!FileCanBeRead(saveTarget))
                {
                    await Task.Delay(500);
                }
            }
            else
            {
                lock (_objCompreLock)
                {
                    // Create file
                    using var stream = File.OpenWrite(saveTarget);
                    // Load and compress file
                    var image = Image.Load(sourceImage, out IImageFormat format);
                    image.Mutate(x => x.AutoOrient());
                    image.Metadata.ExifProfile = null;
                    image.Mutate(x => x
                        .Resize(width, height));
                    image.Save(stream, format);
                    stream.Close();
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
}
