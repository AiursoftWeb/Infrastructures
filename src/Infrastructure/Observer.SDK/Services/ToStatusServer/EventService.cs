using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Models.EventAddressModels;
using Aiursoft.Observer.SDK.Models.EventViewModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Observer.SDK.Services.ToStatusServer
{
    public class EventService : IScopedDependency
    {
        private readonly HTTPService _http;
        private readonly StatusLocator _statusLocation;

        public EventService(
            HTTPService http,
            StatusLocator serviceLocation)
        {
            _http = http;
            _statusLocation = serviceLocation;
        }

        public async Task<AiurProtocol> LogAsync(string accessToken, string message, string stackTrace, EventLevel eventLevel, string path)
        {
            var url = new AiurUrl(_statusLocation.Endpoint, "Event", "Log", new { });
            var form = new AiurUrl(string.Empty, new LogAddressModel
            {
                AccessToken = accessToken,
                Message = message,
                StackTrace = stackTrace,
                EventLevel = eventLevel,
                Path = path
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<ViewLogViewModel> ViewAsync(string accessToken)
        {
            var url = new AiurUrl(_statusLocation.Endpoint, "Event", "View", new ViewAddressModel
            {
                AccessToken = accessToken
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<ViewLogViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }
    }
}
