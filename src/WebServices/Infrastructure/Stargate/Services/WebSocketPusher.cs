using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Services.ToStatusServer;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Services
{
    public class WebSocketPusher : IScopedDependency
    {
        private WebSocket _ws;
        private bool _dropped;
        private readonly AppsContainer _appsContainer;
        private readonly EventService _eventService;

        public bool Connected => !_dropped && _ws.State == WebSocketState.Open;

        public WebSocketPusher(
            AppsContainer appsContainer,
            EventService eventService)
        {
            _appsContainer = appsContainer;
            _eventService = eventService;
        }

        public async Task Accept(HttpContext context)
        {
            _ws = await context.WebSockets.AcceptWebSocketAsync();
        }

        public async Task SendMessage(string message)
        {
            await _ws.SendMessage(message);
        }

        public async Task PendingClose()
        {
            try
            {
                var buffer = new ArraySegment<byte>(new byte[4096 * 20]);
                while (true)
                {
                    await _ws.ReceiveAsync(buffer, CancellationToken.None);
                    if (_ws.State != WebSocketState.Open)
                    {
                        _dropped = true;
                        return;
                    }
                }
            }
            catch (Exception e) when (!e.Message.StartsWith("The remote party closed the WebSocket connection"))
            {
                _dropped = true;
                var accessToken = _appsContainer.AccessToken();
                await _eventService.LogExceptionAsync(await accessToken, e, "InPusher");
            }
        }
    }
}
