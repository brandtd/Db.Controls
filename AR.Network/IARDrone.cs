using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace AR.Drone
{
    public interface IARDrone : INotifyPropertyChanged
    {
        /// <summary>IP address of drone.</summary>
        IPAddress Address { get; }

        float Altitude { get; } // TODO: use real units

        /// <summary>Product hardware version.</summary>
        string HardwareVersion { get; }

        /// <summary>Whether WiFi is in "outdoor" mode.</summary>
        bool OutdoorWifi { get; }

        float Pitch { get; }

        /// <summary>Device's product name.</summary>
        string ProductName { get; }

        float Roll { get; }

        /// <summary>Measured round-trip-time for comms with drone.</summary>
        TimeSpan RoundTripTime { get; }

        /// <summary>RSSI of the signal between controller and the product (in dbm).</summary>
        short RssiInDbMilliWatts { get; }

        /// <summary>Product serial number.</summary>
        string SerialNumber { get; }

        /// <summary>Product software version.</summary>
        string SoftwareVersion { get; }

        /// <summary>Down speed component (in m/s).</summary>
        float SpeedDown { get; } // TODO: use real units

        /// <summary>East speed component (in m/s).</summary>
        float SpeedEast { get; } // TODO: use real units

        /// <summary>North speed component (in m/s).</summary>
        float SpeedNorth { get; } // TODO: use real units

        /// <summary>Date on system (in ISO-8601 format).</summary>
        string SystemDate { get; }

        /// <summary>Time on system (in ISO-8601 format).</summary>
        string SystemTime { get; }

        float Yaw { get; }
    }
}