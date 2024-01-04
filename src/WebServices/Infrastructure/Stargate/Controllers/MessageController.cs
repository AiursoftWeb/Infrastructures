using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models.MessageAddressModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Stargate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class MessageController : ControllerBase
{
    private readonly StargateMemory _memoryContext;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public MessageController(
        StargateMemory memoryContext,
        AiursoftAppTokenValidator tokenManager)
    {
        _memoryContext = memoryContext;
        _tokenManager = tokenManager;
    }

    [HttpPost]
    public async Task<IActionResult> PushMessage(PushMessageAddressModel model)
    {
        //Ensure app
        var appid =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        //Ensure channel
        var channel = _memoryContext.GetChannelsUnderApp(appid).SingleOrDefault(t => t.Id == model.ChannelId);
        if (channel == null)
        {
            return this.Protocol(Code.NotFound, $"We can not find your channel with id: '{model.ChannelId}'!");
        }

        await channel.Messages.BroadcastAsync(model.MessageContent);
        return this.Protocol(new AiurResponse
        {
            Code = Code.JobDone,
            Message = $"You have successfully pushed a new message to channel: {channel.Id}!"
        });
    }
}