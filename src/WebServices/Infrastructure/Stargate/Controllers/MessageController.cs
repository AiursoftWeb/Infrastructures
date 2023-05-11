using System.Linq;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models;
using Aiursoft.Stargate.SDK.Models.MessageAddressModels;
using Aiursoft.XelNaga.Services;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Stargate.Controllers;

[APIExpHandler]
[APIModelStateChecker]
public class MessageController : ControllerBase
{
    private readonly Counter _counter;
    private readonly StargateMemory _memoryContext;
    private readonly ACTokenValidator _tokenManager;

    public MessageController(
        StargateMemory memoryContext,
        Counter counter,
        ACTokenValidator tokenManager)
    {
        _memoryContext = memoryContext;
        _counter = counter;
        _tokenManager = tokenManager;
    }

    [HttpPost]
    public IActionResult PushMessage(PushMessageAddressModel model)
    {
        //Ensure app
        var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
        //Ensure channel
        var channel = _memoryContext.GetChannelsUnderApp(appid).SingleOrDefault(t => t.Id == model.ChannelId);
        if (channel == null)
            return this.Protocol(ErrorType.NotFound, $"We can not find your channel with id: '{model.ChannelId}'!");
        //Create Message
        var message = new Message
        {
            Id = _counter.GetUniqueNo(),
            ChannelId = channel.Id,
            Content = model.MessageContent
        };
        channel.Push(message);
        return this.Protocol(new AiurProtocol
        {
            Code = ErrorType.Success,
            Message = $"You have successfully pushed a new message to channel: {channel.Id}!"
        });
    }
}