using PrimusNetServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimusNetServer.Network.Attributes
{
    public class EngineIOMessageTypeAttribute : Attribute
    {
        public EngineIOMessageTypeAttribute(EngineIOMessageTypeEnum value)
        {
            Value = value;
        }

        public EngineIOMessageTypeEnum Value { get; set; }

    }
}
