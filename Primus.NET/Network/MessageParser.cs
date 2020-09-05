using Primus.NET.Enums;
using Primus.NET.Extensions;
using Primus.NET.Network.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Primus.NET.Network
{
    public class MessageParser
    {
        private static bool _isInitialized = false;
        private static Dictionary<EngineIOMessageTypeEnum, Delegate> _handlersEngineIO = new Dictionary<EngineIOMessageTypeEnum, Delegate>();
        private static Dictionary<string, Delegate> _handlersPrimus = new Dictionary<string, Delegate>();

        public static void Initialize()
        {
            Assembly asm = typeof(MessageParser).Assembly;
            if (!_isInitialized)
            {
                if (PrimusClient.SocketType == SocketTypeEnum.ENGINE_IO)
                {
                    _isInitialized = true;
                    var methodsEIO = asm.GetTypes()
                              .SelectMany(t => t.GetMethods())
                              .Where(m => m.HasAttribute<EngineIOMessageTypeAttribute>());

                    foreach (var method in methodsEIO)
                    {
                        Delegate @delegate = method.CreateDelegate(new[] { typeof(PrimusClient), typeof(string) });
                        EngineIOMessageTypeAttribute attribute = method.GetCustomAttribute<EngineIOMessageTypeAttribute>();

                        _handlersEngineIO.Add(attribute.Value, @delegate);
                    }
                }

                _isInitialized = true;
                var methodsPrimus = asm.GetTypes()
                          .SelectMany(t => t.GetMethods())
                          .Where(m => m.HasAttribute<PrimusMessageTypeAttribute>());

                foreach (var method in methodsPrimus)
                {
                    Delegate @delegate = method.CreateDelegate(new[] { typeof(PrimusClient), typeof(string) });
                    PrimusMessageTypeAttribute attribute = method.GetCustomAttribute<PrimusMessageTypeAttribute>();

                    _handlersPrimus.Add(attribute.Value, @delegate);
                }
            }
        }

        public static async Task Handle(string rawMessage, PrimusClient client)
        {
            if (string.IsNullOrWhiteSpace(rawMessage))
            {
                await client.Close();
                return;
            }

            if (PrimusClient.SocketType == SocketTypeEnum.ENGINE_IO && client != null)
            {
                if (!byte.TryParse(rawMessage[0].ToString(), out byte messageTypeValue))
                {
                    await client.Close();
                    return;
                }

                EngineIOMessageTypeEnum messageType = (EngineIOMessageTypeEnum)messageTypeValue;
                if (messageTypeValue < (int)EngineIOMessageTypeEnum.OPENING || messageTypeValue > (int)EngineIOMessageTypeEnum.HANDSHAKE_COMPLETE || messageType == EngineIOMessageTypeEnum.CLOSE)
                {
                    await client.Close();
                    return;
                }

                rawMessage = rawMessage[1..];

                if (_handlersEngineIO.TryGetValue(messageType, out Delegate handlerEIO))
                {
                    handlerEIO.DynamicInvoke(null, client, rawMessage);
                }

                HandlePrimusMessage(rawMessage, client);
            }
            else
            {
                HandlePrimusMessage(rawMessage, client);
            }
        }

        private static void HandlePrimusMessage(string rawMessage, PrimusClient client)
        {
            var handlerPrimus = _handlersPrimus.FirstOrDefault(handler => rawMessage.Contains(handler.Key)).Value;
            if (handlerPrimus != null)
            {
                handlerPrimus.DynamicInvoke(null, client, rawMessage);
            }
        }
    }
}
