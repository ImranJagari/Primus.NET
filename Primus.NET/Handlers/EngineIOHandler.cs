﻿using Primus.NET.Enums;
using Primus.NET.Network;
using Primus.NET.Network.Attributes;
using System;

namespace Primus.NET.Handlers
{
    public class EngineIOHandler
    {
        [EngineIOMessageType(EngineIOMessageTypeEnum.PING)]
        public static void HandlePingMessage(PrimusClient client, string data)
        {
            client.Pong(data.Contains("probe"));
        }

        [EngineIOMessageType(EngineIOMessageTypeEnum.PONG)]
        public static void HandlePongMessage(PrimusClient client, string _)
        {
            client.LastHeartBeatSent = DateTime.Now;
        }

        [EngineIOMessageType(EngineIOMessageTypeEnum.RECEIVE_SEND)]
        public static void HandleReceiveData(PrimusClient client, string data)
        {
            client.OnDataReceived(data);
        }

        [EngineIOMessageType(EngineIOMessageTypeEnum.HANDSHAKE_COMPLETE)]
        public static void HandleHandshakeComplete(PrimusClient client, string _)
        {
            client.CompleteHandshake();
        }
    }
}
