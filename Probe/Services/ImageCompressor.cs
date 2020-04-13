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

        public ImageCompressor(
            IConfiguration configuration,
            SizeCalculator sizeCalculator)
        {
            _configuration = configuration;
            _sizeCalculator = sizeCalculator;
        }

        public async Task<string> ClearExif(string path)
        {
            try
            {
                var clearedFolder = _configuration["StoragePath"] + $"{Path.DirectorySeparatorChar}ClearedEXIF{Path.DirectorySeparatorChar}";
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
            var fileIsLatest = File.Exists(saveTarget) && new FileInfo(saveTarget).LastWriteTime > new FileInfo(sourceImage).LastWriteTime;
            var fileCanRead = FileCanBeRead(saveTarget);
            if (fileIsLatest && fileCanRead)
            {
                return;
            }
            else if (fileIsLatest && !fileCanRead)
            {
                while (!FileCanBeRead(saveTarget))
                {
                    await Task.Delay(500);
                }
            }
            else
            {
                File.OpenWrite(saveTarget).Close();
                await Task.Run(() =>
                {
                    var image = Image.Load(sourceImage, out IImageFormat format);
                    image.Mutate(x => x.AutoOrient());
                    image.Metadata.ExifProfile = null;
                    using var stream = File.OpenWrite(saveTarget);
                    image.Save(stream, format);
                    stream.Close();
                });
            }
        }

        public async Task<string> Compress(string path, int width, int height)
        {
            if (height == width)
            {
                width = _sizeCalculator.Ceiling(width);
                height = _sizeCalculator.Ceiling(height);
            }
            if (height != width)
            {
                width = _sizeCalculator.Ceiling(width);
                height = 0;
            }
            try
            {
                var compressedFolder = _configuration["StoragePath"] + $"{Path.DirectorySeparatorChar}Compressed{Path.DirectorySeparatorChar}";
                if (Directory.Exists(compressedFolder) == false)
                {
                    Directory.CreateDirectory(compressedFolder);
                }
                var compressedImagePath = $"{compressedFolder}probe_compressed_w{width}_h{height}_fileId_{Path.GetFileNameWithoutExtension(path)}.dat";
                await SaveReducedImage(path, compressedImagePath, width, height);
                return compressedImagePath;
            }
            catch (ImageFormatException)
            {
                return path;
            }
        }

        private async Task SaveReducedImage(string sourceImage, string saveTarget, int width, int height)
        {
            var fileIsLatest = File.Exists(saveTarget) && new FileInfo(saveTarget).LastWriteTime > new FileInfo(sourceImage).LastWriteTime;
            var fileCanRead = FileCanBeRead(saveTarget);
            if (fileIsLatest && fileCanRead)
            {
                return;
            }
            else if (fileIsLatest && !fileCanRead)
            {
                while (!FileCanBeRead(saveTarget))
                {
                    await Task.Delay(500);
                }
            }
            else
            {
                File.OpenWrite(saveTarget).Close();
                await Task.Run(() =>
                {
                    var image = Image.Load(sourceImage, out IImageFormat format);
                    image.Mutate(x => x.AutoOrient());
                    image.Metadata.ExifProfile = null;
                    image.Mutate(x => x
                        .Resize(width, height));
                    using var stream = File.OpenWrite(saveTarget);
                    image.Save(stream, format);
                    stream.Close();
                });
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
