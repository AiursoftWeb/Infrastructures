using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Wrapgate.SDK.Models;
using Aiursoft.Wrapgate.SDK.Models.AddressModels;
using Aiursoft.Wrapgate.SDK.Models.ViewModels;
using Aiursoft.Wrapgate.SDK.Services;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Probe.SDK.Services.ToProbeServer
{
    public class RecordsService : IScopedDependency
    {
        private readonly HTTPService _http;
        private readonly WrapgateLocator _serviceLocation;
        public RecordsService(
            HTTPService http,
            WrapgateLocator serviceLocation)
        {
            _http = http;
            _serviceLocation = serviceLocation;
        }

        public async Task<AiurProtocol> CreateNewRecordAsync(string accessToken, string newRecordName, string targetUrl, RecordType type)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "Records", "CreateNewRecord", new { });
            var form = new AiurUrl(string.Empty, new CreateNewRecordAddressModel
            {
                AccessToken = accessToken,
                NewRecordName = newRecordName,
                TargetUrl = targetUrl,
                Type = type
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewMyRecordsViewModel> ViewMyRecordsAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "Records", "ViewMyRecords", new ViewMyRecordsAddressModel
            {
                AccessToken = accessToken
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<ViewMyRecordsViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> UpdateRecordInfoAsync(string accessToken, string oldRecordName, string newRecordName, RecordType newType, string newUrl)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "Records", "UpdateRecordInfo", new { });
            var form = new AiurUrl(string.Empty, new UpdateRecordInfoAddressModel
            {
                AccessToken = accessToken,
                OldRecordName = oldRecordName,
                NewRecordName = newRecordName,
                NewType = newType,
                NewUrl = newUrl
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> DeleteRecordAsync(string accessToken, string recordName)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "Records", "DeleteRecord", new { });
            var form = new AiurUrl(string.Empty, new DeleteRecordAddressModel
            {
                AccessToken = accessToken,
                RecordName = recordName
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> DeleteAppAsync(string accessToken, string appId)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "Records", "DeleteApp", new { });
            var form = new AiurUrl(string.Empty, new DeleteAppAddressModel
            {
                AccessToken = accessToken,
                AppId = appId
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
