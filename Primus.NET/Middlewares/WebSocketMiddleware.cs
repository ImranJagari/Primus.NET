using Microsoft.AspNetCore.Http;
using Primus.NET.Network;
using System;
using System.Threading.Tasks;

namespace Primus.NET.Middlewares
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var socketManager = context.WebSockets;
            if (socketManager.IsWebSocketRequest && context.Request.Query["transport"] == "websocket" && Guid.TryParse(context.Request.Query["sid"], out Guid clientId))
            {
                var webSocket = await socketManager.AcceptWebSocketAsync();
                {
                    PrimusClient client = ClientManager.GetClient(clientId);
                    if (client != null)
                    {
                        client.ClientWs = webSocket;
                        await client.StartReceive();
                    }
                }
            }
            if (!context.Response.HasStarted)
                await _next(context);
        }
    }
}
