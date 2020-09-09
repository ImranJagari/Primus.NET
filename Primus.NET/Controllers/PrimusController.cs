using Microsoft.AspNetCore.Mvc;
using Primus.NET.Enums;
using Primus.NET.Network;
using Primus.NET.Network.Servers;
using System.Threading.Tasks;

namespace Primus.NET.Controllers
{
    [Route("")]
    public class PrimusController : Controller
    {
        [HttpGet]
        [HttpPost]
        [Route("primus")]
        public async Task<IActionResult> PrimusEIO(string transport, string sid)
        {
            if (PrimusClient.SocketType == SocketTypeEnum.ENGINE_IO)
            {
                return await EngineIOServer.CheckWebsocketContext(HttpContext, transport, sid);
            }
            return Json(string.Empty);
        }

        [HttpGet]
        [Route("primus/{serverId}/{connectionId}/websocket")]
        public async Task<IActionResult> PrimusSockJs(int serverId, string connectionId)
        {
            return await Task.FromResult(Json(new {
                serverId,
                connectionId
            }));
        }

        [HttpGet]
        [Route("primus/primus.js")]
        public string GetPrimusClient()
        {
            var text = System.IO.File.ReadAllText("primus.js");
            return text;
        }
    }
}
