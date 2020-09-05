using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Primus.NET.Network.Attributes
{
    public class PrimusMessageTypeAttribute : Attribute
    {
        public PrimusMessageTypeAttribute(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; set; }
    }
}
