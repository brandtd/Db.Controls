using AR.Drone;
using System;
using System.ComponentModel;

namespace AR.App
{
    /// <summary>View model for a drone.</summary>
    public class DroneViewModel : PropertyChangedBase, IDisposable
    {
        public DroneViewModel(IARDrone drone)
        {
            Model = drone;

            Model.PropertyChanged += onModelChanged;
        }

        /// <summary>The model this view model is wrapped around.</summary>
        public IARDrone Model { get; private set; }

        public float Altitude => Model.Altitude;
        public string HardwareVersion => Model.HardwareVersion;
        public bool OutdoorWifi => Model.OutdoorWifi;
        public float Pitch => Model.Pitch;
        public string ProductName => Model.ProductName;
        public float Roll => Model.Roll;
        public TimeSpan RoundTripTime => Model.RoundTripTime;
        public short RssiInDbMilliWatts => Model.RssiInDbMilliWatts;
        public string SerialNumber => Model.SerialNumber;
        public string SoftwareVersion => Model.SoftwareVersion;
        public float Yaw => Model.Yaw;

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (Model != null)
            {
                Model.PropertyChanged -= onModelChanged;
                Model = null;
            }
        }

        private void onModelChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Model.Altitude): OnPropertyChanged(nameof(Altitude)); break;
                case nameof(Model.Roll): OnPropertyChanged(nameof(Roll)); break;
                case nameof(Model.Pitch): OnPropertyChanged(nameof(Pitch)); break;
                case nameof(Model.Yaw): OnPropertyChanged(nameof(Yaw)); break;
                case nameof(Model.HardwareVersion): OnPropertyChanged(nameof(HardwareVersion)); break;
                case nameof(Model.OutdoorWifi): OnPropertyChanged(nameof(OutdoorWifi)); break;
                case nameof(Model.ProductName): OnPropertyChanged(nameof(ProductName)); break;
                case nameof(Model.RoundTripTime): OnPropertyChanged(nameof(RoundTripTime)); break;
                case nameof(Model.RssiInDbMilliWatts): OnPropertyChanged(nameof(RssiInDbMilliWatts)); break;
                case nameof(Model.SerialNumber): OnPropertyChanged(nameof(SerialNumber)); break;
                case nameof(Model.SoftwareVersion): OnPropertyChanged(nameof(SoftwareVersion)); break;
            }
        }
    }
}