using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public static class FileService
    {
        /// <summary>
        /// This triggers the current action to download a real file storaged in disk.
        /// </summary>
        /// <param name="controller">You must pass current request controller.</param>
        /// <param name="path">The physical path of the file you wanna download.</param>
        /// <param name="filename">Real file name. Will be used to generate correct MIME type.</param>
        /// <param name="download">If set, will let the browser directly download the file.</param>
        /// <param name="suggestedFileName">If `download` was set and this was not empty, will override the file name argument to be the file name after downloading.</param>
        /// <returns></returns>
        public static async Task<IActionResult> AiurFile(this ControllerBase controller, string path, string filename, string suggestedFileName)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var fileInfo = new FileInfo(path);
                var extension = filename.Substring(filename.LastIndexOf('.') + 1);
                long etagHash = fileInfo.LastWriteTime.ToUniversalTime().ToFileTime() ^ fileInfo.Length;
                var _etag = Convert.ToString(etagHash, 16);
                controller.Response.Headers.Add("ETag", '\"' + _etag + '\"');
                if (controller.Request.Headers.Keys.Contains("If-None-Match") && controller.Request.Headers["If-None-Match"].ToString().Trim('\"') == _etag)
                {
                    return new StatusCodeResult(304);
                }
                controller.Response.Headers.Add("Content-Length", fileInfo.Length.ToString());
                return controller.PhysicalFile(path, "application/octet-stream", suggestedFileName, true);
            });
        }

        public static async Task<IActionResult> AiurFile(this ControllerBase controller, string path, string filename)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var fileInfo = new FileInfo(path);
                var extension = filename.LastIndexOf('.') > 0 ? filename.Substring(filename.LastIndexOf('.') + 1) : string.Empty;
                long etagHash = fileInfo.LastWriteTime.ToUniversalTime().ToFileTime() ^ fileInfo.Length;
                var etag = Convert.ToString(etagHash, 16);
                controller.Response.Headers.Add("ETag", '\"' + etag + '\"');
                if (controller.Request.Headers.Keys.Contains("If-None-Match") && controller.Request.Headers["If-None-Match"].ToString().Trim('\"') == etag)
                {
                    return new StatusCodeResult(304);
                }
                controller.Response.Headers.Add("Content-Length", fileInfo.Length.ToString());
                return controller.PhysicalFile(path, MIME.GetContentType(extension), true);
            });
        }
    }
}
