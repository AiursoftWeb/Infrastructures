using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Http;

namespace Aiursoft.Stargate.Services;

public class WebSocketPusher : IScopedDependency
{
    private readonly AppsContainer _appsContainer;
    private readonly ObserverService _eventService;
    private bool _dropped;
    private WebSocket _ws;

    public WebSocketPusher(
        AppsContainer appsContainer,
        ObserverService eventService)
    {
        _appsContainer = appsContainer;
        _eventService = eventService;
    }

    public bool Connected => !_dropped && _ws.State == WebSocketState.Open;

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
                if (_ws.State == WebSocketState.Open)
                {
                    continue;
                }

                _dropped = true;
                return;
            }
        }
        catch (Exception e) when (!e.Message.StartsWith("The remote party closed the WebSocket connection"))
        {
            _dropped = true;
            var accessToken = _appsContainer.GetAccessTokenAsync();
            await _eventService.LogExceptionAsync(await accessToken, e, "InPusher");
        }
    }

    public Task Close()
    {
        if (_ws.State == WebSocketState.Open)
        {
            return _ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close because of error.",
                CancellationToken.None);
        }

        return Task.CompletedTask;
    }
}