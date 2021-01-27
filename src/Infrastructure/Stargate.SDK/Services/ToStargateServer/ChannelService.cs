using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Stargate.SDK.Models.ChannelAddressModels;
using Aiursoft.Stargate.SDK.Models.ChannelViewModels;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.SDK.Services.ToStargateServer
{
    public class ChannelService : IScopedDependency
    {
        private readonly StargateLocator _stargateLocator;
        private readonly APIProxyService _http;

        public ChannelService(
            StargateLocator serviceLocation,
            APIProxyService http)
        {
            _stargateLocator = serviceLocation;
            _http = http;
        }

        public async Task<ViewMyChannelsViewModel> ViewMyChannelsAsync(string accessToken)
        {
            var url = new AiurUrl(_stargateLocator.Endpoint, "Channel", "ViewMyChannels", new ViewMyChannelsAddressModel
            {
                AccessToken = accessToken
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<ViewMyChannelsViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexpectedResponse(jResult);
            return jResult;
        }

        public async Task<AiurValue<string>> ValidateChannelAsync(int id, string key)
        {
            var url = new AiurUrl(_stargateLocator.Endpoint, "Channel", "ValidateChannel", new ChannelAddressModel
            {
                Id = id,
                Key = key
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            return jResult;
        }

        public async Task<CreateChannelViewModel> CreateChannelAsync(string accessToken, string description)
        {
            var url = new AiurUrl(_stargateLocator.Endpoint, "Channel", "CreateChannel", new { });
            var form = new AiurUrl(string.Empty, new CreateChannelAddressModel
            {
                AccessToken = accessToken,
                Description = description
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<CreateChannelViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexpectedResponse(jResult);
            return jResult;
        }
    }
}
