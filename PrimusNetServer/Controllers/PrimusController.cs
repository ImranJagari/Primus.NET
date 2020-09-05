using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrimusNetServer.Extensions;
using PrimusNetServer.Network;
using PrimusNetServer.Network.Servers;
using System.IO;
using Microsoft.AspNetCore.WebUtilities;

namespace PrimusNetServer.Controllers
{
    [Route("")]
    public class PrimusController : Controller
    {
        [HttpGet]
        [HttpPost]
        [Route("primus")]
        public async Task<IActionResult> Index(string transport, string sid)
        {
            if (PrimusClient.SocketType == Enums.SocketTypeEnum.ENGINE_IO)
            {
                return await EngineIOServer.CheckWebsocketContext(this.HttpContext, transport, sid);
            }
            else if(PrimusClient.SocketType == Enums.SocketTypeEnum.RAW)
            {
                return await RawPrimusServer.CheckWebsocketContext(this.HttpContext);
            }
            return Json(string.Empty);
        }

        [HttpGet]
        [Route("config.json")]
        public string GetConfig()
        {
            var text = System.IO.File.ReadAllText("config.json");
            return text;
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
