using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Attributes
{
    public class CompressorResultFilter : ActionFilterAttribute, IResultFilter
    {
        private bool IsImageMIMEType(string mimeType)
        {
            var validImageTypes = new string[]
            {
                "image/bmp",
                "image/jpeg",
                "image/png"
            };
            return validImageTypes.Contains(mimeType?.ToLower());
        }

        private Stream SaveImageViaMIME(Image<Rgba32> image, string mime)
        {
            Stream stream = new MemoryStream();
            switch (mime?.ToLower())
            {
                case "image/bmp":
                    image.SaveAsBmp(stream);
                    break;
                case "image/jpeg":
                    image.SaveAsJpeg(stream);
                    break;
                case "image/png":
                    image.SaveAsPng(stream);
                    break;
            }
            return stream;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (
                int.TryParse(context.HttpContext.Request.Query["w"], out int w) &&
                int.TryParse(context.HttpContext.Request.Query["h"], out int h) &&
                w > 0 &&
                h > 0 &&
                IsImageMIMEType(context.HttpContext.Response.ContentType)
            )
            {
                var image = Image.Load(context.HttpContext.Response.Body);
                image.Mutate(x => x.AutoOrient());
                image.MetaData.ExifProfile = null;
                image.Mutate(x => x
                    .Resize(w, h));
                var stream = SaveImageViaMIME(image, context.HttpContext.Response.ContentType);
                context.Result = new FileStreamResult(stream, context.HttpContext.Response.ContentType);
            }
        }
    }
}
