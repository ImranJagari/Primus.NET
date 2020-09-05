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
            //var socketManager = httpContext.WebSockets;
            //Guid clientId = Guid.Empty;
            //if (socketManager.IsWebSocketRequest)
            //{
            //    socketManager.AcceptWebSocketAsync().Execute(async (webSocket) => {
            //        PrimusClient client = ClientManager.GetClient(clientId);
            //        if (client != null)
            //        {
            //            client.ClientWs = webSocket;
            //            await client.StartReceive();
            //        }
            //    });

            //    return new ContentResult();
            //}

            return new ContentResult();
        }
    }
}
