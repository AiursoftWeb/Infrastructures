using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public static class HTTPMethods
    {
        public static async Task SendRequestAsync(HttpWebRequest request, string message)
        {
            using (var myRequestStream = await request.GetRequestStreamAsync())
            {
                using (var myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("utf-8")))
                {
                    await myStreamWriter.WriteAsync(message);
                    myStreamWriter.Close();
                }
                myRequestStream.Close();
            }
        }
        public static async Task<string> ReadFromResponseAsync(HttpWebRequest request)
        {
            var retString = string.Empty;
            using (var response = await request.GetResponseAsync())
            {
                using (var myResponseStream = response.GetResponseStream())
                {
                    using (var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                    {
                        retString = await myStreamReader.ReadToEndAsync();
                        myStreamReader.Close();
                    }
                    myResponseStream.Close();
                }
                response.Close();
            }
            return retString;
        }
    }
}
