using SixLabors.ImageSharp;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace Aiursoft.OSS.Services
{
    public class ImageCompressor
    {
        private readonly IConfiguration _configuration;

        public ImageCompressor(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Compress(string path, string realname, int width, int height)
        {
            try
            {
                var compressedFolder = _configuration["StoragePath"] + $"{Path.DirectorySeparatorChar}Compressed{Path.DirectorySeparatorChar}";
                if (Directory.Exists(compressedFolder) == false)
                {
                    Directory.CreateDirectory(compressedFolder);
                }
                var compressedImagePath = $"{compressedFolder}oss_compressed_w{width}h{height}{realname}";
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
                var image = Image.Load(sourceImage);
                image.Mutate(x => x.AutoOrient());
                image.MetaData.ExifProfile = null;
                image.Mutate(x => x
                    .Resize(width, height));
                image.Save(saveTarget);
            });
        }
    }
}