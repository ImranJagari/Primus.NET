using Primus.NET.Enums;
using Primus.NET.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemTimer = System.Timers.Timer;

namespace Primus.NET.Network
{
    public class PrimusClient
    {
        public static readonly SocketTypeEnum SocketType = SocketTypeEnum.ENGINE_IO;
        public const int HeartBeatTimeInMilliseconds = 25000;
        private const int HandshakeCheckupMaxTimes = 3;

        public delegate void OnDataReceivedEvent(string data);
        public event OnDataReceivedEvent DataReceived;

        private byte[] _buffer = new byte[8192];
        private bool _hasGetHeartBeatResponse = true;

        private byte _handshakeCheckupTimes = 0;
        private bool _isHandshakeCompleted = false;

        private SystemTimer _timer;

        public Guid Guid { get; }

        public PrimusClient(Guid guid)
        {
            Guid = guid;
            LastHeartBeatSent = DateTime.Now;

            _timer = new SystemTimer(HeartBeatTimeInMilliseconds);
            _timer.Elapsed += async (e, s) =>
            {
                await CheckHeartBeat();
            };
        }

        public DateTime LastHeartBeatSent { get; set; }
        public bool HasGetHeartBeatResponse
        {
            get
            {
                return _hasGetHeartBeatResponse;
            }
            set
            {
                _hasGetHeartBeatResponse = value;
            }
        }
        public WebSocket ClientWs { get; set; }


        public async Task StartReceive()
        {
            while (ClientWs.State == WebSocketState.Open)
            {
                var result = await ClientWs.ReceiveAsync(_buffer, CancellationToken.None);
                if (result.Count > 0)
                {
                    byte[] tempBuffer = new byte[result.Count];
                    Array.Copy(_buffer, tempBuffer, result.Count);

                    string data = Encoding.UTF8.GetString(tempBuffer);
                    Debug.WriteLine($"Data received from WS : {data}");

                    await MessageParser.Handle(data, this);
                }
                else
                {
                    await Close();
                }
            }
        }

        public async Task Send(string data)
        {
            if (SocketType == SocketTypeEnum.ENGINE_IO)
            {
                data = $"4{data}";
            }

            var dataSerialized = Encoding.UTF8.GetBytes(data);

            if (ClientWs != null && ClientWs.State == WebSocketState.Open)
                await ClientWs.SendAsync(dataSerialized, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        protected virtual async Task CheckHeartBeat()
        {
            if (LastHeartBeatSent.AddMilliseconds(HeartBeatTimeInMilliseconds) < DateTime.Now)
            {
                if (!_isHandshakeCompleted)
                {
                    _handshakeCheckupTimes++;
                }

                if (_handshakeCheckupTimes > HandshakeCheckupMaxTimes && !_isHandshakeCompleted)
                {
                    await Close();
                    return;
                }

                if (_hasGetHeartBeatResponse)
                {
                    _hasGetHeartBeatResponse = false;
                    Ping(true);
                }
                else
                {
                    await Close();
                }
            }
        }

        public void Ping(bool serverHeartBeat = false, bool hasProbe = false)
        {
            if (serverHeartBeat)
                Task.WaitAny(Send($"\"primus::ping::{DateTime.Now.GetUnixTimeStamp()}\""));
            else
            {
                if (SocketType == SocketTypeEnum.ENGINE_IO)
                {
                    var value = EngineIOMessageTypeEnum.PING.GetWSValue();

                    if (hasProbe)
                        value += "probe";

                    Task.WaitAny(Send(value));
                }
            }
            
        }
        public void Pong(bool hasProbe = false)
        {
            if (SocketType == SocketTypeEnum.ENGINE_IO)
            {
                var value = EngineIOMessageTypeEnum.PONG.GetWSValue();

                if (hasProbe)
                    value += "probe";

                var heartBeatEngineIo = Encoding.UTF8.GetBytes(value);

                ClientWs.SendAsync(heartBeatEngineIo, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            if (SocketType == SocketTypeEnum.RAW)
            {
                var heartBeatPrimus = Encoding.UTF8.GetBytes($"\"primus::pong::{DateTime.Now.GetUnixTimeStamp()}\"");
                ClientWs.SendAsync(heartBeatPrimus, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public void CompleteHandshake()
        {
            _isHandshakeCompleted = true;
        }

        public void OnDataReceived(string data)
        {
            DataReceived?.Invoke(data);
        }

        public async Task Close()
        {
            _timer.Close();
            _timer = null;

            await Send("\"primus::server::close\"");

            ClientManager.RemoveClient(Guid);
            if (ClientWs != null && ClientWs.State != WebSocketState.Closed)
                await ClientWs.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

            GC.SuppressFinalize(this);

        }
    }
}
