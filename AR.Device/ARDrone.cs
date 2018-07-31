#region MIT License (c) 2018 Dan Brandt

// Copyright 2018 Dan Brandt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion MIT License (c) 2018 Dan Brandt

using AR.Commands;
using AR.Commands.Common.CommonState;
using AR.Commands.Common.SettingsState;
using AR.Commands.Common.WifiSettingsState;
using AR.Network;
using System;
using System.Threading.Tasks;

namespace AR.Device
{
    /// <summary>Base implementation of a drone.</summary>
    public abstract class ARDrone : ARWifiDevice, IARDrone, IDisposable
    {
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

        /// <inheritdoc cref="IARDrone.ProductName" />
        public string ProductName
        {
            get => _productName;
            private set => SetProperty(ref _productName, value);
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

        /// <inheritdoc />
        protected ARDrone(ARCommandCodec codec) : base(codec)
        {
        }

        protected override async Task HandleReceivedCommand(ARCommand command)
        {
            await base.HandleReceivedCommand(command);

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
        }

        private string _hardwareVersion = string.Empty;
        private bool _outdoorWifi;
        private string _productName = string.Empty;
        private short _rssiInDbMilliWatts;
        private string _serialHigh = string.Empty;
        private string _serialLow = string.Empty;
        private string _softwareVersion = string.Empty;
        private string _systemDate = string.Empty;
        private string _systemTime = string.Empty;
    }
}