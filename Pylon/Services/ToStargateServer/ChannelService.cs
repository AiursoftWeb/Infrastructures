using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Stargate.ChannelAddressModels;
using Aiursoft.Pylon.Models.Stargate.ChannelViewModels;
using Aiursoft.Pylon.Models.Stargate.ListenAddressModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToStargateServer
{
    public class ChannelService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public ChannelService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<CreateChannelViewModel> CreateChannelAsync(string AccessToken, string Description)
        {
            var url = new AiurUrl(_serviceLocation.Stargate, "Channel", "CreateChannel", new { });
            var form = new AiurUrl(string.Empty, new CreateChannelAddressModel
            {
                AccessToken = AccessToken,
                Description = Description
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<CreateChannelViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public async Task<AiurProtocol> ValidateChannelAsync(int Id, string Key)
        {
            var url = new AiurUrl(_serviceLocation.Stargate, "Channel", "ValidateChannel", new ChannelAddressModel
            {
                Id = Id,
                Key = Key
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            return jResult;
        }
    }
}
