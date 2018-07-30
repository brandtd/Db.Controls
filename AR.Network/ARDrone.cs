using AR.Commands;
using ARParrot.Commands.Ardrone3.PilotingState;
using ARParrot.Commands.Common.CommonState;
using ARParrot.Commands.Common.Settings;
using ARParrot.Commands.Common.SettingsState;
using ARParrot.Commands.Common.WifiSettingsState;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AR.Drone
{
    /// <summary>Base implementation of a drone.</summary>
    public abstract class ARDrone : PropertyChangedBase, IARDrone, IDisposable
    {
        /// <inheritdoc cref="IARDrone.Address" />
        public IPAddress Address
        {
            get => _address;
            private set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc cref="Altitude" />
        public float Altitude
        {
            get => _altitude;
            set => SetProperty(ref _altitude, value);
        }

        /// <inheritdoc cref="IARDrone.HardwareVersion" />
        public string HardwareVersion
        {
            get => _hardwareVersion;
            set => SetProperty(ref _hardwareVersion, value);
        }

        /// <inheritdoc cref="IARDrone.OutdoorWifi" />
        public bool OutdoorWifi
        {
            get => _outdoorWifi;
            private set => SetProperty(ref _outdoorWifi, value);
        }

        /// <inheritdoc cref="Roll" />
        public float Pitch
        {
            get => _pitch;
            set => SetProperty(ref _pitch, value);
        }

        /// <inheritdoc cref="IARDrone.ProductName" />
        public string ProductName
        {
            get => _productName;
            private set => SetProperty(ref _productName, value);
        }

        /// <inheritdoc cref="Roll" />
        public float Roll
        {
            get => _roll;
            set => SetProperty(ref _roll, value);
        }

        /// <inheritdoc cref="IARDrone.RoundTripTime" />
        public TimeSpan RoundTripTime
        {
            get => _roundTripTime;
            private set => SetProperty(ref _roundTripTime, value);
        }

        /// <inheritdoc cref="IARDrone.RssiInDbMilliWatts" />
        public short RssiInDbMilliWatts
        {
            get => _rssiInDbMilliWatts;
            private set => SetProperty(ref _rssiInDbMilliWatts, value);
        }

        /// <inheritdoc cref="IARDrone.SerialNumber" />
        public string SerialNumber
        {
            get => _serialHigh + _serialLow;
        }

        /// <inheritdoc cref="IARDrone.SoftwareVersion" />
        public string SoftwareVersion
        {
            get => _softwareVersion;
            set => SetProperty(ref _softwareVersion, value);
        }

        /// <inheritdoc cref="SpeedDown" />
        public float SpeedDown
        {
            get => _speedDown;
            set => SetProperty(ref _speedDown, value);
        }

        /// <inheritdoc cref="SpeedEast" />
        public float SpeedEast
        {
            get => _speedEast;
            set => SetProperty(ref _speedEast, value);
        }

        /// <inheritdoc cref="SpeedNorth" />
        public float SpeedNorth
        {
            get => _speedNorth;
            set => SetProperty(ref _speedNorth, value);
        }

        /// <inheritdoc cref="IARDrone.SystemDate" />
        public string SystemDate
        {
            get => _systemDate;
            set => SetProperty(ref _systemDate, value);
        }

        /// <inheritdoc cref="IARDrone.SystemTime" />
        public string SystemTime
        {
            get => _systemTime;
            set => SetProperty(ref _systemTime, value);
        }

        /// <inheritdoc cref="Roll" />
        public float Yaw
        {
            get => _yaw;
            set => SetProperty(ref _yaw, value);
        }

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
        private IPAddress _address;
        private float _altitude;
        private UdpClient _client;
        private CancellationTokenSource _cts;
        private string _hardwareVersion = string.Empty;
        private bool _outdoorWifi;
        private Task _pingTask;
        private float _pitch;
        private string _productName = string.Empty;
        private Task _receiveTask;
        private IPEndPoint _remoteEndPoint;
        private float _roll;
        private TimeSpan _roundTripTime;
        private short _rssiInDbMilliWatts;
        private string _serialHigh = string.Empty;
        private string _serialLow = string.Empty;
        private string _softwareVersion = string.Empty;
        private float _speedDown;
        private float _speedEast;
        private float _speedNorth;
        private string _systemDate = string.Empty;
        private string _systemTime = string.Empty;

        private float _yaw;

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
                    if (command is CmdWifiSignalChanged wifiSignalStatusMsg)
                    {
                        RssiInDbMilliWatts = wifiSignalStatusMsg.Rssi;
                    }
                    else if (command is CmdOutdoorSettingsChanged outdoorSettingsMsg)
                    {
                        OutdoorWifi = outdoorSettingsMsg.Outdoor == 1;
                    }
                    else if (command is CmdProductNameChanged productNameMsg)
                    {
                        ProductName = productNameMsg.Name;
                    }
                    else if (command is CmdProductSerialHighChanged serialHighMsg)
                    {
                        _serialHigh = serialHighMsg.High;
                        OnPropertyChanged(nameof(SerialNumber));
                    }
                    else if (command is CmdProductSerialLowChanged serialLowMsg)
                    {
                        _serialLow = serialLowMsg.Low;
                        OnPropertyChanged(nameof(SerialNumber));
                    }
                    else if (command is CmdProductVersionChanged productMsg)
                    {
                        SoftwareVersion = productMsg.Software;
                        HardwareVersion = productMsg.Hardware;
                    }
                    else if (command is CmdCurrentTimeChanged timeMsg)
                    {
                        SystemTime = timeMsg.Time;
                    }
                    else if (command is CmdCurrentDateChanged dateMsg)
                    {
                        SystemDate = dateMsg.Date;
                    }
                    else if (command is CmdSpeedChanged speedMsg)
                    {
                        // TODO: bebop only
                        SpeedNorth = speedMsg.SpeedX;
                        SpeedEast = speedMsg.SpeedY;
                        SpeedDown = speedMsg.SpeedZ;
                    }
                    else if (command is CmdAltitudeChanged altitudeMsg)
                    {
                        // TODO: bebop only
                        Altitude = (float)altitudeMsg.Altitude;
                    }
                    else if (command is CmdAttitudeChanged attitudeMsg)
                    {
                        // TODO: bebop only
                        Roll = attitudeMsg.Roll;
                        Pitch = attitudeMsg.Pitch;
                        Yaw = attitudeMsg.Yaw;
                    }
                    else
                    {
                        Console.WriteLine($"Rxed: {command.GetType()}");
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
                    await sendDataMessage(new CmdAllSettings());
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

        private async Task sendDataMessage(ARCommand command)
        {
            byte[] buffer = new byte[1024];
            int length = 0;

            if (_codec.Encode(command, buffer, ref length))
            {
                byte[] dataBuffer = buffer.Take(length).ToArray();
                ARFrame frame = new ARFrame
                {
                    Data = dataBuffer,
                    SequenceNumber = ARFrame.NextSequenceNumber(_c2dCommandBuffer),
                    TargetBuffer = _c2dCommandBuffer,
                    Type = ARFrame.DataType.Data,
                };
                byte[] encoded = frame.Encode();
                await _client.SendAsync(encoded, encoded.Length, _remoteEndPoint);
            }
        }
    }
}