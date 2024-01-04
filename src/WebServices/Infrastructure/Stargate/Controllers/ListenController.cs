using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Stargate.Attributes;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.WebTools.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Stargate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class ListenController : ControllerBase
{
    private readonly StargateMemory _memoryContext;

    public ListenController(StargateMemory memoryContext)
    {
        _memoryContext = memoryContext;
    }

    [AiurForceWebSocket]
    public async Task<IActionResult> Channel(ChannelAddressModel model)
    {
        var channel = _memoryContext[model.Id];
        if (channel == null)
        {
            return this.Protocol(Code.NotFound, "Can not find channel with id: " + model.Id);
        }

        if (channel.ConnectKey != model.Key)
        {
            return this.Protocol(new AiurResponse
            {
                Code = Code.Unauthorized,
                Message = "Wrong connection key!"
            });
        }
        
        var pusher = await HttpContext.AcceptWebSocketClient();
        var outSub = channel
            .Messages
            .Subscribe(pusher);
        try
        {
            await pusher.Listen(HttpContext.RequestAborted);
        }
        catch (TaskCanceledException)
        {
            // Ignore. This happens when the client closes the connection.
        }
        finally
        {
            await pusher.Close(HttpContext.RequestAborted);
            outSub.Unsubscribe();
        }

        return this.Protocol(new AiurResponse { Code = Code.UnknownError, Message = "You shouldn't be here." });
    }
}