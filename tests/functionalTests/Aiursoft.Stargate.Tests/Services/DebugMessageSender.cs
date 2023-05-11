using System;
using System.Threading.Tasks;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Stargate.SDK.Services.ToStargateServer;

namespace Aiursoft.Stargate.Tests.Services;

public class DebugMessageSender : IScopedDependency
{
    private readonly PushMessageService _messageService;

    public DebugMessageSender(PushMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task SendDebuggingMessages(string accessToken, int channelId)
    {
        for (var i = 0; i < 100; i++)
        {
            await _messageService.PushMessageAsync(accessToken, channelId, new
            {
                Guid = Guid.NewGuid().ToString("N"),
                Time = DateTime.UtcNow,
                Id = i
            });
            await Task.Delay(1);
        }
    }
}