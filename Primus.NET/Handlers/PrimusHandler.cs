﻿using Microsoft.AspNetCore.Builder;
using Primus.NET.Network;
using Primus.NET.Network.Attributes;
using System.Threading.Tasks;

namespace Primus.NET.Handlers
{
    public class PrimusHandler
    {
        [PrimusMessageType("primus::pong")]
        public static void HandlePrimusPing(PrimusClient client, string _)
        {
            client.HasGetHeartBeatResponse = true;
        }

        [PrimusMessageType("primus::id")]
        public static void HandlePrimusId(PrimusClient client, string _)
        {
            Task.WaitAny(client.Send($"\"primus::id::{client.Guid}\""));
        }
    }
}
