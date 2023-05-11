using System.Threading.Tasks;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Stargate.SDK.Models.MessageAddressModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aiursoft.Stargate.SDK.Services.ToStargateServer;

public class PushMessageService : IScopedDependency
{
    private readonly APIProxyService _httpService;
    private readonly StargateLocator _stargateLocator;

    public PushMessageService(
        APIProxyService httpService,
        StargateLocator serviceLocation)
    {
        _httpService = httpService;
        _stargateLocator = serviceLocation;
    }

    public async Task<AiurProtocol> PushMessageAsync(string accessToken, int channelId, object eventObject)
    {
        var url = new AiurUrl(_stargateLocator.Endpoint, "Message", "PushMessage", new { });
        var payloadToken = JsonConvert.SerializeObject(eventObject, new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        var form = new AiurUrl(string.Empty, new PushMessageAddressModel
        {
            AccessToken = accessToken,
            ChannelId = channelId,
            MessageContent = payloadToken
        });
        var result = await _httpService.Post(url, form, true);
        var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
        if (jResult.Code != ErrorType.Success) throw new AiurUnexpectedResponse(jResult);
        return jResult;
    }
}