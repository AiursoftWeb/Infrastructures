using Aiursoft.CSTools.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.Services;

public static class FileService
{
    private static (string etag, long length) GetFileHTTPProperties(string path)
    {
        var fileInfo = new FileInfo(path);
        var etagHash = fileInfo.LastWriteTime.ToUniversalTime().ToFileTime() ^ fileInfo.Length;
        var etag = Convert.ToString(etagHash, 16);
        return (etag, fileInfo.Length);
    }

    public static IActionResult WebFile(this ControllerBase controller, string path, string extension)
    {
        var (etag, length) = GetFileHTTPProperties(path);
        // Handle etag
        controller.Response.Headers.Append("ETag", '\"' + etag + '\"');
        if (controller.Request.Headers.Keys.Contains("If-None-Match"))
        {
            if (controller.Request.Headers["If-None-Match"].ToString().Trim('\"') == etag)
            {
                return new StatusCodeResult(304);
            }
        }

        // Return file result.
        controller.Response.Headers.Append("Content-Length", length.ToString());
        // Allow cache
        controller.Response.Headers.Append("Cache-Control", $"public, max-age={TimeSpan.FromDays(7).TotalSeconds}");
        return controller.PhysicalFile(path, Mime.GetContentType(extension), true);
    }
}