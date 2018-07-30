using AR.Commands;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AR.Drone
{
    public abstract class ARDrone : IDisposable
    {
        public IPAddress Address { get; private set; }

        /// <summary>Connect with drone at given address/port.</summary>
        public virtual async Task<string> Connect(IPAddress address, ushort port)
        {
            string error = string.Empty;
            byte[] responseBuffer = new byte[1024];

            TcpClient tcpClient = new TcpClient();
            UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            try
            {
                await tcpClient.ConnectAsync(address, port);
                NetworkStream stream = tcpClient.GetStream();

                byte[] request = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ARConnectionRequest
                {
                    ControllerName = System.AppDomain.CurrentDomain.FriendlyName,
                    ControllerType = "PC",
                    D2cPort = (udpClient.Client.LocalEndPoint as IPEndPoint).Port,
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
                _client = udpClient;
                _cts = new CancellationTokenSource();
                _receiveTask = receiveContinuousAsync(_cts.Token);
            }
            else
            {
                udpClient.Dispose();
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
        private Task _receiveTask;

        private async Task handleIfAckRequested(ARFrame frame, IPEndPoint remoteEndpoint)
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

                await _client.SendAsync(response, response.Length, remoteEndpoint);
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
                    Console.WriteLine($"Received command of type: {command.GetType()}");
                }
            }

            return Task.CompletedTask;
        }

        private async Task handleIfHeartbeat(ARFrame frame, IPEndPoint remoteEndpoint)
        {
            // Data sent to the '0' should be bounced back immediately so ping can be measured.
            if (frame.TargetBuffer == 0)
            {
                byte[] response = new ARFrame
                {
                    Type = ARFrame.DataType.Data,
                    TargetBuffer = 1,
                    SequenceNumber = ARFrame.NextSequenceNumber(1),
                    Data = frame.Data,
                }.Encode();

                await _client.SendAsync(response, response.Length, remoteEndpoint);

                Console.WriteLine("Bounced back packet");
            }
        }

        private async Task receiveContinuousAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result = await _client.ReceiveAsync();

                    int index = 0;
                    while (index < result.Buffer.Length)
                    {
                        ARFrame frame = ARFrame.Decode(result.Buffer, ref index);

                        if (frame != null)
                        {
                            await handleIfHeartbeat(frame, result.RemoteEndPoint);
                            await handleIfAckRequested(frame, result.RemoteEndPoint);
                            await handleIfCommandTypeReceived(frame);
                        }
                        else
                        {
                            index = result.Buffer.Length;
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}