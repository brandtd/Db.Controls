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

using DotSpatial.Positioning;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AR.Device
{
    public interface IARBebop : IARDrone
    {
        /// <summary>Drone's altitude (WGS-84).</summary>
        Distance Altitude { get; }

        /// <summary>The current altitude ceiling of the drone.</summary>
        Distance AltitudeCeiling { get; }

        /// <summary>Max allowed value for <see cref="AltitudeCeiling" />.</summary>
        Distance AltitudeCeilingRangeMax { get; }

        /// <summary>Min allowed value for <see cref="AltitudeCeiling" />.</summary>
        Distance AltitudeCeilingRangeMin { get; }

        /// <summary>Drone's height above its home location.</summary>
        Distance HeightAboveHome { get; }

        /// <summary>Drone's latitude.</summary>
        Angle Latitude { get; }

        /// <summary>Drone's longitude.</summary>
        Angle Longitude { get; }

        /// <summary>Amount of time to allow to pass before drone will auto recover.</summary>
        TimeSpan LostCommsReturnHomeDelay { get; }

        /// <summary>Drone's max allowed climb rate.</summary>
        Speed MaxClimbRate { get; }

        /// <summary>Max allowed value for <see cref="MaxClimbRate" />.</summary>
        Speed MaxClimbRateRangeMax { get; }

        /// <summary>Min allowed value for <see cref="MaxClimbRate" />.</summary>
        Speed MaxClimbRateRangeMin { get; }

        /// <summary>Whether the <see cref="MaxDistanceFromHome" /> is enabled.</summary>
        bool MaxDistanceEnabled { get; }

        /// <summary>Drone's max allowed distance from home location.</summary>
        Distance MaxDistanceFromHome { get; }

        /// <summary>Max allowed value for <see cref="MaxDistanceFromHome" />.</summary>
        Distance MaxDistanceFromHomeRangeMax { get; }

        /// <summary>Min allowed value for <see cref="MaxDistanceFromHome" />.</summary>
        Distance MaxDistanceFromHomeRangeMin { get; }

        /// <summary>Drone's pitch angle.</summary>
        Angle Pitch { get; }

        /// <summary>Drone's return home behavior.</summary>
        ReturnHomeBehavior ReturnHomeBehavior { get; }

        /// <summary>Drone's roll angle.</summary>
        Angle Roll { get; }

        /// <summary>Down speed component.</summary>
        Speed SpeedDown { get; }

        /// <summary>East speed component.</summary>
        Speed SpeedEast { get; }

        /// <summary>North speed component.</summary>
        Speed SpeedNorth { get; }

        /// <summary>Drone's yaw angle.</summary>
        Angle Yaw { get; }

        /// <summary>Sets the maximum flight distance (from home position) for the drone.</summary>
        Task<bool> SetMaxDistance(Distance maxDistance);
    }
}