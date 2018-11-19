using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public static class StringOperation
    {
        public static string BytesToBase64(this byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        public static byte[] Base64ToBytes(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new byte[0];
            }
            return Convert.FromBase64String(input);
        }

        public static byte[] StringToBytes(this string input)
        {
            return new UTF8Encoding().GetBytes(input);
        }

        public static string BytesToString(this byte[] input)
        {
            return new UTF8Encoding().GetString(input, 0, input.Length);
        }

        public static string StringToBase64(this string input)
        {
            return BytesToBase64(StringToBytes(input));
        }

        public static string Base64ToString(this string input)
        {
            return BytesToString(Base64ToBytes(input));
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            foreach (var c in data)
            {
                sBuilder.Append(c.ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string GetMD5(this string sourceString)
        {
            string hash = GetMd5Hash(MD5.Create(), sourceString);
            return hash;
        }

        public static string GetMD5(this byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                string hex = BitConverter.ToString(hash);
                return hex.Replace("-", "");
            }
        }

        public static string OTake(this string source, int count)
        {
            if (source.Length <= count)
            {
                return source;
            }
            else
            {
                return source.Substring(0, count - 3) + "...";
            }
        }

        public static bool IsInFollowingExtension(this string filename, params string[] extensions)
        {
            var ext = System.IO.Path.GetExtension(filename);
            foreach (var extension in extensions)
            {
                if (ext.Trim('.').ToLower() == extension)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsImageMedia(this string filename)
        {
            return filename.IsInFollowingExtension("jpg", "png", "bmp", "jpeg", "gif", "svg", "ico");
        }

        public static bool IsStaticImage(this string filename)
        {
            return filename.IsInFollowingExtension("jpg", "png", "bmp", "jpeg");
        }

        public static bool IsVideo(this string filename)
        {
            return filename.IsInFollowingExtension("mp4", "webm", "ogg");
        }

        public static string ORemoveHTML(this string content)
        {
            string s = string.Empty;
            content = WebUtility.HtmlDecode(content);
            if (!content.Contains(">"))
            {
                return content;
            }
            while (content.Contains(">"))
            {
                s = s + content.Substring(0, content.IndexOf("<"));
                content = content.Substring(content.IndexOf(">") + 1);
            }
            return s + content;
        }

        private static Random _staticRan { get; set; } = new Random();
        public static string RandomString(int count)
        {
            int number;
            string checkCode = string.Empty;
            var random = new Random(_staticRan.Next());
            for (int i = 0; i < count; i++)
            {
                number = random.Next();
                number = number % 36;
                if (number < 10)
                {
                    number += 48;
                }
                else
                {
                    number += 55;
                }
                checkCode += ((char)number).ToString();
            }
            return checkCode;
        }

        public static string TrimController(this string controllerName)
        {
            return controllerName
                .Replace("Controller", "")
                .Replace("Api", "API");
        }

        public static string SplitStringUpperCase(this string source)
        {
            string[] split = Regex.Split(source, @"(?<!^)(?=[A-Z])");
            var b = new StringBuilder();
            bool first = true;
            foreach (var word in split)
            {
                if (first)
                {
                    b.Append(word + " ");
                    first = false;
                }
                else
                {
                    b.Append(word.ToLower() + " ");
                }
            }
            return b.ToString();
        }
    }
}
