﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Primus.NET.Enums
{
    public enum EngineIOMessageTypeEnum
    {
        OPENING,
        CLOSE,
        PING,
        PONG,
        RECEIVE_SEND,
        HANDSHAKE_COMPLETE,
    }
}
