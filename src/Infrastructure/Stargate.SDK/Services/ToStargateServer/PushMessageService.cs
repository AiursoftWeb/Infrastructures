using System.Threading.Tasks;

using Aiursoft.AiurProtocol.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.Stargate.SDK.Configuration;
using Aiursoft.Stargate.SDK.Models.MessageAddressModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aiursoft.Stargate.SDK.Services.ToStargateServer;

public class PushMessageService : IScopedDependency
{
    private readonly AiurProtocolClient  _httpService;
    private readonly StargateConfiguration _stargateLocator;

    public PushMessageService(
        AiurProtocolClient  httpService,
        IOptions<StargateConfiguration> serviceLocation)
    {
        _httpService = httpService;
        _stargateLocator = serviceLocation.Value;
    }

    public async Task<AiurResponse> PushMessageAsync(string accessToken, int channelId, object eventObject)
    {
        var url = new AiurApiEndpoint(_stargateLocator.Instance, "Message", "PushMessage", new { });
        var payloadToken = JsonConvert.SerializeObject(eventObject, new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        var form = new ApiPayload( new PushMessageAddressModel
        {
            AccessToken = accessToken,
            ChannelId = channelId,
            MessageContent = payloadToken
        });
        var result = await _httpService.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurResponse>(result);
        if (jResult.Code != ErrorType.Success)
        {
            throw new AiurUnexpectedResponse(jResult);
        }

        return jResult;
    }
}