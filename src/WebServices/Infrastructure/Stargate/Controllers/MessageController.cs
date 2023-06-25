using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models;
using Aiursoft.Stargate.SDK.Models.MessageAddressModels;
using Aiursoft.CSTools.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Stargate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class MessageController : ControllerBase
{
    private readonly Counter _counter;
    private readonly StargateMemory _memoryContext;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public MessageController(
        StargateMemory memoryContext,
        Counter counter,
        AiursoftAppTokenValidator tokenManager)
    {
        _memoryContext = memoryContext;
        _counter = counter;
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

        //Create Message
        var message = new Message
        {
            Id = _counter.GetUniqueNo(),
            ChannelId = channel.Id,
            Content = model.MessageContent
        };
        channel.Push(message);
        return this.Protocol(new AiurResponse
        {
            Code = Code.JobDone,
            Message = $"You have successfully pushed a new message to channel: {channel.Id}!"
        });
    }
}