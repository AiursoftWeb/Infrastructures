using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Aiursoft.Pylon.Services
{
    public static class FileService
    {
        private static (string etag, long length) GetFileHTTPProperties(string path)
        {
            var fileInfo = new FileInfo(path);
            long etagHash = fileInfo.LastWriteTime.ToUniversalTime().ToFileTime() ^ fileInfo.Length;
            var etag = Convert.ToString(etagHash, 16);
            return (etag, fileInfo.Length);
        }

        public static IActionResult WebFile(this ControllerBase controller, string path, string extension)
        {
            var (etag, length) = GetFileHTTPProperties(path);
            // Handle etag
            controller.Response.Headers.Add("ETag", '\"' + etag + '\"');
            if (controller.Request.Headers.Keys.Contains("If-None-Match") && controller.Request.Headers["If-None-Match"].ToString().Trim('\"') == etag)
            {
                return new StatusCodeResult(304);
            }
            // Return file result.
            controller.Response.Headers.Add("Content-Length", length.ToString());
            return controller.PhysicalFile(path, MIME.GetContentType(extension), true);
        }
    }
}
