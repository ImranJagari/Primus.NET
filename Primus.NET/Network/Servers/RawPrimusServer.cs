using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Primus.NET.Network.Servers
{
    public class RawPrimusServer
    {
        //not tested
        public static async Task<IActionResult> CheckWebsocketContext(HttpContext httpContext)
        {
            var socketManager = httpContext.WebSockets;
            if (socketManager.IsWebSocketRequest)
            {
                var webSocket = await socketManager.AcceptWebSocketAsync();
                {
                    PrimusClient client = ClientManager.CreateClient();
                    if (client != null)
                    {
                        client.ClientWs = webSocket;
                        await client.StartReceive();
                    }
                }

                return new ContentResult();
            }
            return new ContentResult();
        }
    }
}
