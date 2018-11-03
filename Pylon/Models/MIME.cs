using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models
{
    public class MIME
    {
        public static string GetContentType(string extenstion)
        {
            string lower = extenstion.ToLower().TrimStart('.');
            //Not to download the file, and we can process the file, let us process it.
            if (_mimeTypesDictionary.ContainsKey(lower))
            {
                Console.WriteLine(extenstion);
                return _mimeTypesDictionary[lower];
            }
            return "application/octet-stream";
        }
        private static readonly Dictionary<string, string> _mimeTypesDictionary = new Dictionary<string, string>
        {
            {"avi", "video/x-msvideo"},
            {"apk","application/vnd.android.package-archive"},
            {"bmp", "image/bmp"},
            {"css", "text/css"},
            {"dll", "application/octet-stream"},
            {"doc", "application/msword"},
            {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {"gif", "image/gif"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"ico", "image/x-icon"},
            {"jpeg", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"js", "application/x-javascript"},
            {"map","text/plain"},
            {"m4a", "audio/mp4a-latm"},
            {"mid", "audio/midi"},
            {"mov", "video/quicktime"},
            {"mp3", "audio/mpeg"},
            {"md", "text/markdown; charset=UTF-8"},
            {"webm","video/webm"},
            {"mp4", "video/mp4"},
            {"mpeg", "video/mpeg"},
            {"mpg", "video/mpeg"},
            {"ogg", "application/ogg"},
            {"pdf", "application/pdf"},
            {"png", "image/png"},
            {"ppt", "application/vnd.ms-powerpoint"},
            {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
            {"swf", "application/x-shockwave-flash"},
            {"svg", "image/svg+xml"},
            {"tif", "image/tiff"},
            {"txt", "text/plain"},
            {"xhtml", "application/xhtml+xml"},
            {"xls", "application/vnd.ms-excel"},
            {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {"zip", "application/zip"},
            {"iso", "application/iso" },
            {"7z","application/x-7z-compressed" },
            {"rtf", "text/rtf"},
            {"m4u", "video/vnd.mpegurl"},
            {"tiff", "image/tiff"},
            {"woff","application/x-font-woff"},
            {"woff2","application/x-font-woff2"},
            {"ttf","application/x-font-truetype"},
            {"otf","application/x-font-opentype"},
            {"eot","application/application/vnd.ms-fontobject"},
            {"ts","application/x-typescript"}
        };
    }
}
