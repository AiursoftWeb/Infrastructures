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
        public static async Task<CreateChannelViewModel> CreateChannelAsync(string AccessToken, string Description)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.StargateServerAddress, "Channel", "CreateChannel", new { });
            var form = new AiurUrl(string.Empty, new CreateChannelAddressModel
            {
                AccessToken = AccessToken,
                Description = Description
            });
            var result = await httpContainer.Post(url, form);
            var jResult = JsonConvert.DeserializeObject<CreateChannelViewModel>(result);
            if (jResult.code != ErrorType.Success)
                throw new AiurUnexceptedResponse(jResult);
            return jResult;
        }

        public static async Task<AiurProtocal> ValidateChannelAsync(int Id, string Key)
        {
            var httpContainer = new HTTPService();
            var url = new AiurUrl(Values.StargateServerAddress, "Channel", "ValidateChannel", new ChannelAddressModel
            {
                Id = Id,
                Key = Key
            });
            var result = await httpContainer.Get(url);
            var jResult = JsonConvert.DeserializeObject<AiurProtocal>(result);
            return jResult;
        }
    }
}
