using AR.Commands;
using ARParrot.Commands.Common.CommonState;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AR.Drone
{
    /// <summary>Base implementation of a drone.</summary>
    public abstract class ARDrone : IARDrone, IDisposable
    {
        /// <inheritdoc cref="IARDrone.Address" />
        public IPAddress Address { get; private set; }

        /// <inheritdoc cref="IARDrone.RoundTripTime" />
        public TimeSpan RoundTripTime { get; private set; }

        /// <inheritdoc cref="IARDrone.RssiInDbMilliWatts" />
        public short RssiInDbMilliWatts { get; private set; }

        /// <summary>Connect with drone at given address/port.</summary>
        public virtual async Task<string> Connect(IPAddress address, ushort port)
        {
            string error = string.Empty;
            byte[] responseBuffer = new byte[1024];

            TcpClient tcpClient = new TcpClient();
            _client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            try
            {
                await tcpClient.ConnectAsync(address, port);
                NetworkStream stream = tcpClient.GetStream();

                byte[] request = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ARConnectionRequest
                {
                    ControllerName = System.AppDomain.CurrentDomain.FriendlyName,
                    ControllerType = "PC",
                    D2cPort = (_client.Client.LocalEndPoint as IPEndPoint).Port,
                }));
                await stream.WriteAsync(request, 0, request.Length);
                int read = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);

                if (read > 0)
                {
                    ARConnectionResponse response = JsonConvert.DeserializeObject<ARConnectionResponse>(Encoding.UTF8.GetString(responseBuffer));

                    if (response.Status != 0)
                    {
                        error = $"Status error: {response.Status}";
                    }
                    else
                    {
                        _remoteEndPoint = new IPEndPoint(address, response.C2dPort);
                    }
                }
            }
            catch (Exception ex) when (
                ex is ArgumentNullException ||
                ex is ArgumentOutOfRangeException ||
                ex is SocketException)
            {
                error = $"Unable to connect with drone: {ex.Message}";
            }
            catch (JsonException ex)
            {
                error = $"JSON error: {ex.Message}";
            }

            if (error == string.Empty)
            {
                Address = address;
                _cts = new CancellationTokenSource();
                _receiveTask = receiveContinuousAsync();
                _pingTask = pingTask();
            }
            else
            {
                _client.Dispose();
                _client = null;
            }
            tcpClient.Dispose();

            return error;
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public virtual void Dispose()
        {
            _client?.Dispose();
            _cts?.Cancel();
        }

        /// <summary>Use given codec to encode/decode commands.</summary>
        protected ARDrone(ARCommandCodec codec)
        {
            _codec = codec;
        }

        private const int _c2dCommandBuffer = 10;
        private const int _c2dCommandWithAckBuffer = 11;
        private const int _c2dEmergencyCommandBuffer = 12;
        private const int _c2dStreamAckBuffer = 13;
        private const int _d2cCommandBuffer = 127;
        private const int _d2cCommandWithAckBuffer = 126;
        private const int _d2cStreamBuffer = 125;
        private readonly ARCommandCodec _codec;
        private UdpClient _client;
        private CancellationTokenSource _cts;
        private Task _pingTask;
        private Task _receiveTask;
        private IPEndPoint _remoteEndPoint;

        private async Task handleIfAckRequested(ARFrame frame)
        {
            // 'Ack' the data, if requested.
            if (frame.Type == ARFrame.DataType.DataWithAck)
            {
                byte[] response = new ARFrame
                {
                    Type = ARFrame.DataType.Ack,
                    SequenceNumber = ARFrame.NextSequenceNumber(0x80 | frame.TargetBuffer),
                    TargetBuffer = (byte)(0x80 | frame.TargetBuffer),
                    Data = new byte[] { frame.SequenceNumber },
                }.Encode();

                await _client.SendAsync(response, response.Length, _remoteEndPoint);
            }
        }

        private Task handleIfCommandTypeReceived(ARFrame frame)
        {
            // Received a command data type.
            if (frame.TargetBuffer == _d2cCommandBuffer ||
                frame.TargetBuffer == _d2cCommandWithAckBuffer)
            {
                int dataIndex = 0;
                ARCommand command = _codec.Decode(frame.Data, ref dataIndex);
                if (command != null)
                {
                    if (command is CmdWifiSignalChanged wifiSignalStatus)
                    {
                        RssiInDbMilliWatts = wifiSignalStatus.Rssi;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private async Task handleIfHeartbeat(ARFrame frame)
        {
            if (frame.TargetBuffer == 0)
            {
                // Data sent to the '0' should be bounced back immediately so ping can be measured.
                byte[] response = new ARFrame
                {
                    Type = ARFrame.DataType.Data,
                    TargetBuffer = 1,
                    SequenceNumber = ARFrame.NextSequenceNumber(1),
                    Data = frame.Data,
                }.Encode();

                await _client.SendAsync(response, response.Length, _remoteEndPoint);

                Console.WriteLine($"Bounced back packet (Seq: {frame.SequenceNumber})");
            }
            else if (frame.TargetBuffer == 1)
            {
                // Data received in buffer '1' is our timestamp bounced back to us.
                long currentTime = Stopwatch.GetTimestamp();
                long sentTime = BitConverter.ToInt64(frame.Data, 0);

                long numTicks = currentTime - sentTime;
                RoundTripTime = TimeSpan.FromSeconds(numTicks / (double)Stopwatch.Frequency);
            }
        }

        private async Task pingTask()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    long sendTime = Stopwatch.GetTimestamp();
                    ARFrame frame = new ARFrame
                    {
                        Data = BitConverter.GetBytes(sendTime),
                        SequenceNumber = ARFrame.NextSequenceNumber(0),
                        TargetBuffer = 0,
                        Type = ARFrame.DataType.Data,
                    };
                    byte[] data = frame.Encode();
                    await _client.SendAsync(data, data.Length, _remoteEndPoint);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected exception: {ex.Message}");
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }

        private async Task receiveContinuousAsync()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    UdpReceiveResult result = await _client.ReceiveAsync();

                    int index = 0;
                    while (index < result.Buffer.Length)
                    {
                        ARFrame frame = ARFrame.Decode(result.Buffer, ref index);

                        if (frame != null)
                        {
                            await handleIfHeartbeat(frame);
                            await handleIfAckRequested(frame);
                            await handleIfCommandTypeReceived(frame);
                        }
                        else
                        {
                            index = result.Buffer.Length;
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected exception: {ex.Message}");
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }
    }
}