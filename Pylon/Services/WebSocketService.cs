using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;

namespace Aiursoft.Pylon.Services
{
    public static class WebSocketService
    {
        public static async Task SendMessage(this WebSocket ws, string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
