using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe.TokenAddressModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToProbeServer
{
    public class TokenService : IScopedDependency
    {
        private readonly HTTPService _http;
        private readonly ServiceLocation _serviceLocation;

        public TokenService(
            HTTPService http,
            ServiceLocation serviceLocation)
        {
            _http = http;
            _serviceLocation = serviceLocation;
        }

        public async Task<string> GetUploadTokenAsync(string accessToken, string siteName, string permissions, string underPath)
        {
            var url = new AiurUrl(_serviceLocation.Probe, "Token", "GetUploadToken", new { });
            var form = new AiurUrl(string.Empty, new GetUploadTokenAddressModel
            {
                AccessToken = accessToken,
                SiteName = siteName,
                Permissions = permissions,
                UnderPath = underPath
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult.Value;
        }
    }
}
