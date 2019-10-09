using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Services
{
    public class WebSocketPusher : IScopedDependency
    {
        private WebSocket _ws;
        public bool Connected => _ws.State == WebSocketState.Open;

        public async Task Accept(HttpContext context)
        {
            _ws = await context.WebSockets.AcceptWebSocketAsync();
        }

        public async Task SendMessage(string message)
        {
            await _ws.SendMessage(message);
        }
    }
}
