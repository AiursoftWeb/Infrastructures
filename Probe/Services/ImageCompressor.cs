using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class ImageCompressor
    {
        private readonly IConfiguration _configuration;

        public ImageCompressor(
            IConfiguration configuration)
        {
            _configuration = configuration;
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
                await GetClearedImage(path, clearededImagePath);
                return clearededImagePath;
            }
            catch (ImageFormatException)
            {
                return path;
            }
        }

        private async Task GetClearedImage(string sourceImage, string saveTarget)
        {
            var sourceFileInfo = new FileInfo(sourceImage);
            if (File.Exists(saveTarget))
            {
                if (new FileInfo(saveTarget).LastWriteTime > sourceFileInfo.LastWriteTime)
                {
                    return;
                }
            }
            await Task.Run(() =>
            {
                var image = Image.Load(sourceImage, out IImageFormat format);
                image.Mutate(x => x.AutoOrient());
                image.Metadata.ExifProfile = null;
                using (var stream = File.OpenWrite(saveTarget))
                {
                    image.Save(stream, format);
                    stream.Close();
                }
            });
        }

        public async Task<string> Compress(string path, int width, int height)
        {
            try
            {
                var compressedFolder = _configuration["StoragePath"] + $"{Path.DirectorySeparatorChar}Compressed{Path.DirectorySeparatorChar}";
                if (Directory.Exists(compressedFolder) == false)
                {
                    Directory.CreateDirectory(compressedFolder);
                }
                var compressedImagePath = $"{compressedFolder}probe_compressed_w{width}_h{height}_fileId_{Path.GetFileNameWithoutExtension(path)}.dat";
                await GetReducedImage(path, compressedImagePath, width, height);
                return compressedImagePath;
            }
            catch (ImageFormatException)
            {
                return path;
            }
        }

        private async Task GetReducedImage(string sourceImage, string saveTarget, int width, int height)
        {
            var sourceFileInfo = new FileInfo(sourceImage);
            if (File.Exists(saveTarget))
            {
                if (new FileInfo(saveTarget).LastWriteTime > sourceFileInfo.LastWriteTime)
                {
                    return;
                }
            }
            await Task.Run(() =>
            {
                var image = Image.Load(sourceImage, out IImageFormat format);
                image.Mutate(x => x.AutoOrient());
                image.Metadata.ExifProfile = null;
                image.Mutate(x => x
                    .Resize(width, height));
                using (var stream = File.OpenWrite(saveTarget))
                {
                    image.Save(stream, format);
                    stream.Close();
                }
            });
        }
    }
}
