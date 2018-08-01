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
using AR.Commands.Common.SettingsState;
using AR.Device;
using AR.Network;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AR.App
{
    public class MainWindowViewModel : PropertyChangedBase, IDisposable
    {
        public MainWindowViewModel()
        {
            _codec = new ARCommandCodec();
            _codec.RegisterCommands(typeof(ARCommand).Assembly);
            _codec.RegisterCommands(typeof(CmdAllSettingsChanged).Assembly);

            _droneNetwork = new ARNetwork();
            _droneNetwork.BebopDiscovered += onBebopDiscovered;
            _droneNetwork.BebopLost += onBebopLost;
            _droneNetwork.StartSearching();
        }

        public ObservableCollection<DroneViewModel> Drones { get; } = new ObservableCollection<DroneViewModel>();

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (_droneNetwork != null)
            {
                _droneNetwork.Dispose();
                _droneNetwork = null;
            }
        }

        private ARCommandCodec _codec;
        private ARNetwork _droneNetwork;

        private async void onBebopDiscovered(object sender, ARNetwork.ServiceDiscoveredArgs e)
        {
            ARBebop bebop = new ARBebop(_codec);
            string error = await bebop.Connect(e.Address, e.Port);
            if (string.IsNullOrEmpty(error))
            {
                Drones.Add(new DroneViewModel(bebop));
            }
        }

        private void onBebopLost(object sender, ARNetwork.ServiceLostArgs e)
        {
            DroneViewModel toRemove = null;
            foreach (DroneViewModel drone in Drones)
            {
                toRemove = Drones.FirstOrDefault(d => d.Model.Address == e.Address);
            }

            if (toRemove != null)
            {
                Drones.Remove(toRemove);
            }
        }
    }
}