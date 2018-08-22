using Aiursoft.Pylon.Models.Stargate;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToStargateServer;
using Aiursoft.Stargate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Services
{
    public class Debugger
    {
        private readonly PushMessageService _messageService;
        public Debugger(PushMessageService messageService)
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
