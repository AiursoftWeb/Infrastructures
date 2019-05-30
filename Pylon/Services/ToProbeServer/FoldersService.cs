using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Models.Probe.FoldersAddressModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToProbeServer
{
    public class FoldersService
    {
        private readonly HTTPService _http;
        private readonly ServiceLocation _serviceLocation;
        public FoldersService(
            HTTPService http,
            ServiceLocation serviceLocation)
        {
            _http = http;
            _serviceLocation = serviceLocation;
        }

        public async Task<AiurValue<Folder>> ViewContentAsync(string accessToken, string siteName, string folderNames)
        {
            var url = new AiurUrl(_serviceLocation.Probe, $"/Folders/ViewContent/{siteName}/{folderNames?.TrimStart('/')}", new ViewContentAddressModel
            {
                AccessToken = accessToken,
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurValue<Folder>>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> CreateNewFolderAsync(string accessToken, string siteName, string folderNames, string newFolderName)
        {
            var url = new AiurUrl(_serviceLocation.Probe, $"/Folders/CreateNewFolder/{siteName}/{folderNames}", new { });
            var form = new AiurUrl(string.Empty, new CreateNewFolderAddressModel
            {
                AccessToken = accessToken,
                NewFolderName = newFolderName
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> DeleteFolderAsync(string accessToken, string siteName, string folderNames)
        {
            var url = new AiurUrl(_serviceLocation.Probe, $"/Folders/DeleteFolder/{siteName}/{folderNames}", new { });
            var form = new AiurUrl(string.Empty, new DeleteFolderAddressModel
            {
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
