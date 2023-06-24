using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Services;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.Stargate.SDK.Configuration;
using Aiursoft.Stargate.SDK.Models.ChannelAddressModels;
using Aiursoft.Stargate.SDK.Models.ChannelViewModels;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Microsoft.Extensions.Options;

namespace Aiursoft.Stargate.SDK.Services.ToStargateServer;

public class ChannelService : IScopedDependency
{
    private readonly AiurProtocolClient _http;
    private readonly StargateConfiguration _stargateLocator;

    public ChannelService(
        IOptions<StargateConfiguration> serviceLocation,
        AiurProtocolClient http)
    {
        _stargateLocator = serviceLocation.Value;
        _http = http;
    }

    public async Task<ViewMyChannelsViewModel> ViewMyChannelsAsync(string accessToken)
    {
        var url = new AiurApiEndpoint(_stargateLocator.Instance, "Channel", "ViewMyChannels", new ViewMyChannelsAddressModel
        {
            AccessToken = accessToken
        });
        return await _http.Get<ViewMyChannelsViewModel>(url);
    }

    public async Task<AiurValue<string>> ValidateChannelAsync(int id, string key)
    {
        var url = new AiurApiEndpoint(_stargateLocator.Instance, "Channel", "ValidateChannel", new ChannelAddressModel
        {
            Id = id,
            Key = key
        });
        return await _http.Get<AiurValue<string>>(url);
    }

    public async Task<CreateChannelViewModel> CreateChannelAsync(string accessToken, string description)
    {
        var url = new AiurApiEndpoint(_stargateLocator.Instance, "Channel", "CreateChannel", new { });
        var form = new AiurApiPayload( new CreateChannelAddressModel
        {
            AccessToken = accessToken,
            Description = description
        });
        return await _http.Post<CreateChannelViewModel>(url, form);
    }
}