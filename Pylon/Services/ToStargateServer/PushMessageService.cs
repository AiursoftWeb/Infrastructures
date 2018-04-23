using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Stargate.MessageAddressModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToStargateServer
{
    public class PushMessageService
    {
        private readonly HTTPService _httpService;
        private readonly ServiceLocation _serviceLocation;
        public PushMessageService(
            HTTPService httpService,
            ServiceLocation serviceLocation)
        {
            _httpService = httpService;
            _serviceLocation = serviceLocation;
        }
        public async Task<AiurProtocal> PushMessageAsync(string AccessToken, int ChannelId, string MessageContent, bool noexception = false)
        {
            var url = new AiurUrl(_serviceLocation.Stargate, "Message", "PushMessage", new { });
            var form = new AiurUrl(string.Empty, new PushMessageAddressModel
            {
                AccessToken = AccessToken,
                ChannelId = ChannelId,
                MessageContent = MessageContent
            });
            var result = await _httpService.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (!noexception && jResult.Code != ErrorType.Success)
            {
                throw new AiurUnexceptedResponse(jResult);
            }
            return jResult;
        }
    }
}
