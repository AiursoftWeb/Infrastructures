using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class HTTPService
    {
        public HTTPService()
        {

        }
        public CookieContainer CC = new CookieContainer();

        public async Task<string> Get(AiurUrl Url)
        {
            var request = WebRequest.CreateHttp(Url.ToString());
            request.CookieContainer = CC;
            request.Method = "GET";
            request.ContentType = "text/html;charset=utf-8";
            return await HTTPMethods.ReadFromResponseAsync(request);
        }
        public async Task<string> Post(AiurUrl Url, AiurUrl postDataStr)
        {
            var request = WebRequest.CreateHttp(Url.ToString());
            request.CookieContainer = CC;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            await HTTPMethods.SendRequestAsync(request, postDataStr.ToString().TrimStart('?'));
            return await HTTPMethods.ReadFromResponseAsync(request);
        }

        public async Task<string> PostFile(AiurUrl Url, string filepath)
        {
            var request = new HttpClient();
            var form = new MultipartFormDataContent();
            HttpResponseMessage response = null;
            using (var memory = new MemoryStream())
            {
                using (var fileStream = new FileStream(filepath, mode: FileMode.Open))
                {
                    await fileStream.CopyToAsync(memory);
                    fileStream.Close();
                }
                memory.Position = 0;
                form.Add(new StreamContent(memory), "file", new FileInfo(filepath).FullName);
                response = await request.PostAsync(Url.ToString(), form);
                memory.Close();
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}
