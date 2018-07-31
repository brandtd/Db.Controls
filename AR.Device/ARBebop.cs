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
using AR.Commands.Ardrone3.GPSSettings;
using AR.Commands.Ardrone3.GPSSettingsState;
using AR.Commands.Ardrone3.PilotingSettingsState;
using AR.Commands.Ardrone3.PilotingState;
using AR.Commands.Ardrone3.SpeedSettingsState;
using DotSpatial.Positioning;
using System;
using System.Threading.Tasks;

namespace AR.Device
{
    /// <summary>Interface for a bebop drone.</summary>
    public class ARBebop : ARDrone, IARBebop
    {
        /// <inheritdoc />
        public ARBebop(ARCommandCodec codec) : base(codec)
        {
        }

        /// <inheritdoc cref="IARBebop.Altitude" />
        public Distance Altitude
        {
            get => _altitude;
            set => SetProperty(ref _altitude, value);
        }

        /// <inheritdoc cref="IARBebop.AltitudeCeiling" />
        public Distance AltitudeCeiling
        {
            get => _altitudeCeiling;
            set => SetProperty(ref _altitudeCeiling, value);
        }

        /// <inheritdoc cref="IARBebop.AltitudeCeilingRangeMax" />
        public Distance AltitudeCeilingRangeMax
        {
            get => _altitudeCeilingRangeMax;
            set => SetProperty(ref _altitudeCeilingRangeMax, value);
        }

        /// <inheritdoc cref="IARBebop.AltitudeCeilingRangeMin" />
        public Distance AltitudeCeilingRangeMin
        {
            get => _altitudeCeilingRangeMin;
            set => SetProperty(ref _altitudeCeilingRangeMin, value);
        }

        /// <inheritdoc cref="IARBebop.LostCommsReturnHomeDelay" />
        public TimeSpan LostCommsReturnHomeDelay
        {
            get => _lostCommsReturnHomeDelay;
            set => SetProperty(ref _lostCommsReturnHomeDelay, value);
        }

        /// <inheritdoc cref="IARBebop.MaxClimbRate" />
        public Speed MaxClimbRate
        {
            get => _maxClimbRate;
            set => SetProperty(ref _maxClimbRate, value);
        }

        /// <inheritdoc cref="IARBebop.MaxClimbRateRangeMax" />
        public Speed MaxClimbRateRangeMax
        {
            get => _maxClimbRateRangeMax;
            set => SetProperty(ref _maxClimbRateRangeMax, value);
        }

        /// <inheritdoc cref="IARBebop.MaxClimbRateRangeMin" />
        public Speed MaxClimbRateRangeMin
        {
            get => _maxClimbRateRangeMin;
            set => SetProperty(ref _maxClimbRateRangeMin, value);
        }

        /// <inheritdoc cref="IARBebop.MaxDistanceEnabled" />
        public bool MaxDistanceEnabled
        {
            get => _maxDistanceEnabled;
            set => SetProperty(ref _maxDistanceEnabled, value);
        }

        /// <inheritdoc cref="IARBebop.MaxDistanceFromHome" />
        public Distance MaxDistanceFromHome
        {
            get => _maxDistanceFromHome;
            set => SetProperty(ref _maxDistanceFromHome, value);
        }

        /// <inheritdoc cref="IARBebop.MaxDistanceFromHomeRangeMax" />
        public Distance MaxDistanceFromHomeRangeMax
        {
            get => _maxDistanceFromHomeRangeMax;
            set => SetProperty(ref _maxDistanceFromHomeRangeMax, value);
        }

        /// <inheritdoc cref="IARBebop.MaxDistanceFromHomeRangeMin" />
        public Distance MaxDistanceFromHomeRangeMin
        {
            get => _maxDistanceFromHomeRangeMin;
            set => SetProperty(ref _maxDistanceFromHomeRangeMin, value);
        }

        /// <inheritdoc cref="IARBebop.Pitch" />
        public Angle Pitch
        {
            get => _pitch;
            set => SetProperty(ref _pitch, value);
        }

        /// <inheritdoc cref="IARBebop.ReturnHomeBehavior" />
        public ReturnHomeBehavior ReturnHomeBehavior
        {
            get => _returnHomeBehavior;
            set
            {
                if (value != _returnHomeBehavior)
                {
                    _returnHomeBehavior = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc cref="IARBebop.Roll" />
        public Angle Roll
        {
            get => _roll;
            set => SetProperty(ref _roll, value);
        }

        /// <inheritdoc cref="IARBebop.SpeedDown" />
        public Speed SpeedDown
        {
            get => _speedDown;
            set => SetProperty(ref _speedDown, value);
        }

        /// <inheritdoc cref="IARBebop.SpeedEast" />
        public Speed SpeedEast
        {
            get => _speedEast;
            set => SetProperty(ref _speedEast, value);
        }

        /// <inheritdoc cref="IARBebop.SpeedNorth" />
        public Speed SpeedNorth
        {
            get => _speedNorth;
            set => SetProperty(ref _speedNorth, value);
        }

        /// <inheritdoc cref="IARBebop.Yaw" />
        public Angle Yaw
        {
            get => _yaw;
            set => SetProperty(ref _yaw, value);
        }

        protected override async Task HandleReceivedCommand(ARCommand command)
        {
            await base.HandleReceivedCommand(command);

            if (command is CmdSpeedChanged speedMsg)
            {
                SpeedNorth = Speed.FromMetersPerSecond(speedMsg.SpeedX);
                SpeedEast = Speed.FromMetersPerSecond(speedMsg.SpeedY);
                SpeedDown = Speed.FromMetersPerSecond(speedMsg.SpeedZ);
            }
            else if (command is CmdAltitudeChanged altitudeMsg)
            {
                Altitude = Distance.FromMeters(altitudeMsg.Altitude);
            }
            else if (command is CmdAttitudeChanged attitudeMsg)
            {
                Roll = Angle.FromRadians(attitudeMsg.Roll);
                Pitch = Angle.FromRadians(attitudeMsg.Pitch);
                Yaw = Angle.FromRadians(attitudeMsg.Yaw);
            }
            else if (command is CmdMaxAltitudeChanged maxAltitudeMsg)
            {
                AltitudeCeiling = Distance.FromMeters(maxAltitudeMsg.Current);
                AltitudeCeilingRangeMax = Distance.FromMeters(maxAltitudeMsg.Max);
                AltitudeCeilingRangeMin = Distance.FromMeters(maxAltitudeMsg.Min);
            }
            else if (command is CmdMaxDistanceChanged maxDistanceMsg)
            {
                MaxDistanceFromHome = Distance.FromMeters(maxDistanceMsg.Current);
                MaxDistanceFromHomeRangeMax = Distance.FromMeters(maxDistanceMsg.Max);
                MaxDistanceFromHomeRangeMin = Distance.FromMeters(maxDistanceMsg.Min);
            }
            else if (command is CmdNoFlyOverMaxDistanceChanged geoFenceMsg)
            {
                MaxDistanceEnabled = geoFenceMsg.ShouldNotFlyOver == 1;
            }
            else if (command is CmdMaxVerticalSpeedChanged climbRateMsg)
            {
                MaxClimbRate = Speed.FromMetersPerSecond(climbRateMsg.Current);
                MaxClimbRateRangeMax = Speed.FromMetersPerSecond(climbRateMsg.Max);
                MaxClimbRateRangeMin = Speed.FromMetersPerSecond(climbRateMsg.Min);
            }
            else if (command is CmdReturnHomeDelay returnHomeDelayMsg)
            {
                LostCommsReturnHomeDelay = TimeSpan.FromSeconds(returnHomeDelayMsg.Delay);
            }
            else if (command is CmdHomeTypeChanged homeTypeMsg)
            {
                switch (homeTypeMsg.Type)
                {
                    case CmdHomeTypeChanged.TypeEnum.TAKEOFF:
                        ReturnHomeBehavior = ReturnHomeBehavior.TakeOff;
                        break;

                    case CmdHomeTypeChanged.TypeEnum.PILOT:
                        ReturnHomeBehavior = ReturnHomeBehavior.Pilot;
                        break;

                    case CmdHomeTypeChanged.TypeEnum.FOLLOWEE:
                        ReturnHomeBehavior = ReturnHomeBehavior.FollowMe;
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Rxed: {command.GetType()}");
            }
        }

        private Distance _altitude = Distance.Invalid;
        private Distance _altitudeCeiling = Distance.Invalid;
        private Distance _altitudeCeilingRangeMax = Distance.Invalid;
        private Distance _altitudeCeilingRangeMin = Distance.Invalid;
        private TimeSpan _lostCommsReturnHomeDelay;
        private Speed _maxClimbRate = Speed.Invalid;
        private Speed _maxClimbRateRangeMax = Speed.Invalid;
        private Speed _maxClimbRateRangeMin = Speed.Invalid;
        private bool _maxDistanceEnabled;
        private Distance _maxDistanceFromHome = Distance.Invalid;
        private Distance _maxDistanceFromHomeRangeMax = Distance.Invalid;
        private Distance _maxDistanceFromHomeRangeMin = Distance.Invalid;
        private Angle _pitch = Angle.Invalid;
        private ReturnHomeBehavior _returnHomeBehavior = ReturnHomeBehavior.Unknown;
        private Angle _roll = Angle.Invalid;
        private Speed _speedDown = Speed.Invalid;
        private Speed _speedEast = Speed.Invalid;
        private Speed _speedNorth = Speed.Invalid;
        private Angle _yaw = Angle.Invalid;
    }
}