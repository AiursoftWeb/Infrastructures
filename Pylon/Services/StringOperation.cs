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
        public static string GetMD5(this string SourceString)
        {
            string hash = GetMd5Hash(MD5.Create(), SourceString);
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
        public static string OTake(this string source, int Count)
        {
            if (source.Length <= Count)
            {
                return source;
            }
            else
            {
                return source.Substring(0, Count - 3) + "...";
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
        public static string ORemoveHTML(this string Content)
        {
            string s = string.Empty;
            Content = WebUtility.HtmlDecode(Content);
            if (!Content.Contains(">"))
            {
                return Content;
            }
            while (Content.Contains(">"))
            {
                s = s + Content.Substring(0, Content.IndexOf("<"));
                Content = Content.Substring(Content.IndexOf(">") + 1);
            }
            return s + Content;
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
        public static string FormatTimeAgo(TimeSpan ToFormat)
        {
            if (ToFormat.TotalMinutes < 1)
            {
                return "Just now";
            }
            else if (ToFormat.TotalHours < 1)
            {
                return (int)ToFormat.TotalMinutes + " minutes ago";
            }
            else if (ToFormat.TotalDays < 1)
            {
                return (int)ToFormat.TotalHours + " hours ago";
            }
            else if (ToFormat.TotalDays < 30)
            {
                return (int)ToFormat.TotalDays + " day ago";
            }
            else
            {
                return (int)ToFormat.TotalDays / 30 + " months ago";
            }
        }
    }

}
