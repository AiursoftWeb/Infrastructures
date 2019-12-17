using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Services.ToStargateServer;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Services
{
    public class DebugMessageSender : IScopedDependency
    {
        private readonly PushMessageService _messageService;
        public DebugMessageSender(PushMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendDebuggingMessages(string accessToken, int channelId)
        {
            var random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                await _messageService.PushMessageAsync(accessToken, channelId, Guid.NewGuid().ToString("N"));
                await Task.Delay(10);
            }
        }
    }
}
