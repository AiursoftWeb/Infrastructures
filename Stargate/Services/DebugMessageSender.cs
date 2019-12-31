using Aiursoft.SDK.Services.ToStargateServer;
using Aiursoft.XelNaga.Interfaces;
using Newtonsoft.Json;
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
            for (int i = 0; i < 1000; i++)
            {
                var json = JsonConvert.SerializeObject(new
                {
                    Guid = Guid.NewGuid().ToString("N"),
                    Time = DateTime.UtcNow,
                    Id = i
                });
                await _messageService.PushMessageAsync(accessToken, channelId, json);
                await Task.Delay(10);
            }
        }
    }
}
