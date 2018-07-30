using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AR.Drone
{
    public interface IARDrone
    {
        /// <summary>IP address of drone.</summary>
        IPAddress Address { get; }

        /// <summary>Measured round-trip-time for comms with drone.</summary>
        TimeSpan RoundTripTime { get; }

        /// <summary>RSSI of the signal between controller and the product (in dbm).</summary>
        short RssiInDbMilliWatts { get; }
    }
}