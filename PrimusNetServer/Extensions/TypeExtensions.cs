using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace PrimusNetServer.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<T>(this MethodInfo method)
        {
            return method.CustomAttributes.Count(x => x.AttributeType == typeof(T)) > 0;
        }
    }
}