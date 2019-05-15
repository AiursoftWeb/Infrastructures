using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.FilesAddressModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToProbeServer
{
    public class FilesService
    {
        private readonly HTTPService _http;
        private readonly ServiceLocation _serviceLocation;
        public FilesService(
            HTTPService http,
            ServiceLocation serviceLocation)
        {
            _http = http;
            _serviceLocation = serviceLocation;
        }

        public async Task<AiurProtocol> UploadFileAsync(string accessToken, string siteName, string folderNames, string localFilePath)
        {
            var url = new AiurUrl(_serviceLocation.Probe, $"Files/UploadFile/{siteName}/{folderNames}", new UploadFileAddressModel
            {
                AccessToken = accessToken
            });
            var result = await _http.PostFile(url, localFilePath);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
