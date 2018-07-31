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

using AR.Network;

namespace AR.Device
{
    public interface IARDrone : IARWifiDevice
    {
        /// <summary>Product hardware version.</summary>
        string HardwareVersion { get; }

        /// <summary>Whether WiFi is in "outdoor" mode.</summary>
        bool OutdoorWifi { get; }

        /// <summary>Device's product name.</summary>
        string ProductName { get; }

        /// <summary>RSSI of the signal between controller and the product (in dbm).</summary>
        short RssiInDbMilliWatts { get; }

        /// <summary>Product serial number.</summary>
        string SerialNumber { get; }

        /// <summary>Product software version.</summary>
        string SoftwareVersion { get; }

        /// <summary>Date on system (in ISO-8601 format).</summary>
        string SystemDate { get; }

        /// <summary>Time on system (in ISO-8601 format).</summary>
        string SystemTime { get; }
    }
}