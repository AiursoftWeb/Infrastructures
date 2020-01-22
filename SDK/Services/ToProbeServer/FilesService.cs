using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Models.Probe.FilesAddressModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.SDK.Services.ToProbeServer
{
    public class FilesService : IScopedDependency
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

        public async Task<AiurProtocol> DeleteFileAsync(string accessToken, string siteName, string folderNames)
        {
            var url = new AiurUrl(_serviceLocation.Probe, $"/Files/DeleteFile/{siteName.ToUrlEncoded()}/{folderNames.EncodePath()}", new { });
            var form = new AiurUrl(string.Empty, new DeleteFileAddressModel
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
