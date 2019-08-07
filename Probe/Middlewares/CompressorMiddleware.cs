using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Middlewares
{
    public class CompressorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CompressorMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<CompressorMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        private bool IsImageMIMEType(string mimeType)
        {
            var validImageTypes = new string[]
            {
                "image/bmp",
                "image/jpeg",
                "image/png",
                "image/tiff"
            };
            return validImageTypes.Contains(mimeType?.ToLower());
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
            if(
                int.TryParse(context.Request.Query["w"], out int w) &&
                int.TryParse(context.Request.Query["h"], out int h) && 
                w > 0 && 
                h > 0 &&
                IsImageMIMEType(context.Response.ContentType)
            )
            {
                var image = Image.Load(context.Response.Body);
                image.Mutate(x => x.AutoOrient());
                image.MetaData.ExifProfile = null;
                image.Mutate(x => x
                    .Resize(w, h));
                context.Response.Body.Flush();
                context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
                image.SaveAsPng(context.Response.Body);
            }
        }
    }
}
