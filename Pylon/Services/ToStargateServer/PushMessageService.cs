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
        public PushMessageService(HTTPService httpService)
        {
            _httpService = httpService;
        }
        public async Task<AiurProtocal> PushMessageAsync(string AccessToken, int ChannelId, string MessageContent, bool noexception = false)
        {
            var url = new AiurUrl(Values.StargateServerAddress, "Message", "PushMessage", new { });
            var form = new AiurUrl(string.Empty, new PushMessageAddressModel
            {
                AccessToken = AccessToken,
                ChannelId = ChannelId,
                MessageContent = MessageContent
            });
            var result = await _httpService.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            if (!noexception && jResult.code != ErrorType.Success)
            {
                throw new AiurUnexceptedResponse(jResult);
            }
            return jResult;
        }
    }
}
