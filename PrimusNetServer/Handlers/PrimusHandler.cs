using PrimusNetServer.Network;
using PrimusNetServer.Network.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimusNetServer.Handlers
{
    public class PrimusHandler
    {
        [PrimusMessageType("primus::ping")]
        public static void HandlePrimusPing(PrimusClient client, string _)
        {
            client.Pong();
        }

        [PrimusMessageType("primus::id")]
        public static void HandlePrimusId(PrimusClient client, string _)
        {
            Task.WaitAny(client.Send($"\"primus::id::{client.Guid}\""));
        }
    }
}
