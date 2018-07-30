using AR.Drone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AR.App
{
    /// <summary>View model for a drone.</summary>
    public class DroneViewModel : PropertyChangedBase, IDisposable
    {
        public DroneViewModel(IARDrone drone)
        {
            Model = drone;

            _timer = new Timer(TimeSpan.FromMilliseconds(33).TotalMilliseconds);
            _timer.Elapsed += onTimerElapsed;
            _timer.Start();

            RoundTripTime = Model.RoundTripTime;
            RssiInDbMilliWatts = Model.RssiInDbMilliWatts;
        }

        /// <summary>The model this view model is wrapped around.</summary>
        public IARDrone Model { get; private set; }

        /// <summary>Round trip time for communication with drone.</summary>
        public TimeSpan RoundTripTime
        {
            get => _roundTripTime;
            set => SetProperty(ref _roundTripTime, value);
        }

        /// <summary>RSSI of the signal between controller and the product (in dbm).</summary>
        public short RssiInDbMilliWatts
        {
            get => _rssi;
            set => SetProperty(ref _rssi, value);
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= onTimerElapsed;
                _timer.Dispose();
                _timer = null;
            }

            Model = null;
        }

        private TimeSpan _roundTripTime = TimeSpan.Zero;
        private short _rssi = 0;
        private Timer _timer;

        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RoundTripTime = Model.RoundTripTime;
            RssiInDbMilliWatts = Model.RssiInDbMilliWatts;
        }
    }
}