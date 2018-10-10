using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public static class StringOperation
    {
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

        public static bool IsImage(this string filename)
        {
            var AvaliableExtensions = new string[] { "jpg", "png", "bmp" };
            var ext = System.IO.Path.GetExtension(filename);
            foreach (var extension in AvaliableExtensions)
            {
                if (ext.Trim('.').ToLower() == extension)
                {
                    return true;
                }
            }
            return false;
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
    }
}
