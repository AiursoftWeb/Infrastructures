using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private readonly CookieContainer _cc;

        public HTTPService(ILogger<HTTPService> logger)
        {
            _logger = logger;
            _cc = new CookieContainer();
        }

        public async Task<string> Get(AiurUrl url, bool internalRequest = false)
        {
            HttpWebRequest request = null;
            if (internalRequest)
            {
                url.Address = url.Address.Replace("https://", "http://");
                request = WebRequest.CreateHttp(url.ToString());
                request.Headers.Add("x-forwarded-proto", "INTERNAL");
            }
            else
            {
                request = WebRequest.CreateHttp(url.ToString());
            }
            _logger.LogInformation($"Creating HTTP GET request to: {request.RequestUri.ToString()}");
            request.CookieContainer = _cc;
            request.Method = "GET";
            request.ContentType = "text/html;charset=utf-8";
            return await HTTPMethods.ReadFromResponseAsync(request);
        }
        public async Task<string> Post(AiurUrl url, AiurUrl postDataStr, bool internalRequest = false)
        {
            HttpWebRequest request = null;
            if (internalRequest)
            {
                url.Address = url.Address.Replace("https://", "http://");
                request = WebRequest.CreateHttp(url.ToString());
                request.Headers.Add("x-forwarded-proto", "INTERNAL");
            }
            else
            {
                request = WebRequest.CreateHttp(url.ToString());
            }
            _logger.LogInformation($"Creating HTTP Post request to: {request.RequestUri.ToString()}");
            request.CookieContainer = _cc;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            await HTTPMethods.SendRequestAsync(request, postDataStr.ToString().TrimStart('?'));
            return await HTTPMethods.ReadFromResponseAsync(request);
        }

        public async Task<string> PostFile(AiurUrl url, string filepath)
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
                response = await request.PostAsync(url.ToString(), form);
                memory.Close();
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}
