using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Primus.NET.Enums;
using Primus.NET.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Primus.NET.Network.Servers
{
    public class EngineIOServer
    {
        public static async Task<IActionResult> CheckWebsocketContext(HttpContext httpContext, string transport, string sid)
        {
            var socketManager = httpContext.WebSockets;
            Guid clientId = Guid.Empty;
            if (socketManager.IsWebSocketRequest && transport == "websocket" && Guid.TryParse(sid, out clientId))
            {
                socketManager.AcceptWebSocketAsync().Execute(async (webSocket) =>
                {
                    PrimusClient client = ClientManager.GetClient(clientId);
                    if (client != null)
                    {
                        client.ClientWs = webSocket;
                        await client.StartReceive();
                    }
                });

                return new ContentResult();
            }
            else if (transport == "polling" && string.IsNullOrWhiteSpace(sid) && httpContext.Request.Method == "GET")
            {
                PrimusClient client = ClientManager.CreateClient();

                var json = JsonSerializer.Serialize(new
                {
                    sid = client.Guid.ToString(),
                    upgrades = new string[] { "websocket" },
                    pingInterval = PrimusClient.HeartBeatTimeInMilliseconds,
                    pingTimeout = PrimusClient.HeartBeatTimeInMilliseconds / 5,
                });

                return new ContentResult
                {
                    Content = $"{json.Count() + 1}:{EngineIOMessageTypeEnum.OPENING.GetWSValue()}{json}",
                };
            }
            else if (httpContext.Request.Method == "POST" && Guid.TryParse(sid, out clientId))
            {
                PrimusClient client = ClientManager.GetClient(clientId);
                if (client != null)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = "ok",
                    };

                    var request = httpContext.Request;
                    var stream = new StreamReader(request.Body);
                    var body = await stream.ReadToEndAsync();

                    Regex regex = new Regex(@":\S+");
                    var match = regex.Match(body);
                    await MessageParser.Handle(match.Value[1..], client);

                    return result;
                }
            }


            return GetUnknownSessionId();
        }

        private static ContentResult GetUnknownSessionId()
        {
            ContentResult result;
            var json = JsonSerializer.Serialize(new
            {
                code = 1,
                message = "Session ID unknown"
            });

            result = new ContentResult
            {
                Content = json,
                ContentType = "application/json"
            };
            return result;
        }
    }
}
