using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Models.Probe.FoldersAddressModels;
using Newtonsoft.Json;
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
            var url = new AiurUrl(_serviceLocation.Probe, $"/Folders/ViewContent/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new ViewContentAddressModel
            {
                AccessToken = accessToken,
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurValue<Folder>>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> CreateNewFolderAsync(string accessToken, string siteName, string folderNames, string newFolderName, bool recursiveCreate)
        {
            var url = new AiurUrl(_serviceLocation.Probe, $"/Folders/CreateNewFolder/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
            var form = new AiurUrl(string.Empty, new CreateNewFolderAddressModel
            {
                AccessToken = accessToken,
                NewFolderName = newFolderName,
                RecursiveCreate = recursiveCreate
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> DeleteFolderAsync(string accessToken, string siteName, string folderNames)
        {
            var url = new AiurUrl(_serviceLocation.Probe, $"/Folders/DeleteFolder/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
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
