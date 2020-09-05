using PrimusNetServer.Network;
using PrimusNetServer.Network.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimusNetServer.Handlers
{
    public class EngineIOHandler
    {
        [EngineIOMessageType(Enums.EngineIOMessageTypeEnum.PING)]
        public static void HandlePingMessage(PrimusClient client, string data)
        {
            client.Pong(data.Contains("probe"));
        }

        [EngineIOMessageType(Enums.EngineIOMessageTypeEnum.PONG)]
        public static void HandlePongMessage(PrimusClient client, string _)
        {
            client.LastHeartBeatSent = DateTime.Now;
        }

        [EngineIOMessageType(Enums.EngineIOMessageTypeEnum.RECEIVE_SEND)]
        public static void HandleReceiveData(PrimusClient client, string data)
        {
            client.OnDataReceived(data);
        }

        [EngineIOMessageType(Enums.EngineIOMessageTypeEnum.HANDSHAKE_COMPLETE)]
        public static void HandleHandshakeComplete(PrimusClient client, string _)
        {
            client.CompleteHandshake();
        }
    }
}
