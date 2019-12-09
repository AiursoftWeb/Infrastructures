using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Stargate.MessageAddressModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services.ToStargateServer
{
    public class PushMessageService : IScopedDependency
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

        public async Task<AiurProtocol> PushMessageAsync(string accessToken, int channelId, string messageContent, bool noexception = false)
        {
            var url = new AiurUrl(_serviceLocation.Stargate, "Message", "PushMessage", new { });
            var form = new AiurUrl(string.Empty, new PushMessageAddressModel
            {
                AccessToken = accessToken,
                ChannelId = channelId,
                MessageContent = messageContent
            });
            var result = await _httpService.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (!noexception && jResult.Code != ErrorType.Success)
            {
                throw new AiurUnexceptedResponse(jResult);
            }
            return jResult;
        }
    }
}
