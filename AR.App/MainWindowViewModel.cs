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

using AR.Drone;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;

namespace AR.App
{
    public class MainWindowViewModel : PropertyChangedBase, IDisposable
    {
        public MainWindowViewModel()
        {
            _droneNetwork = new ARNetwork();
            ((INotifyCollectionChanged)_droneNetwork.Drones).CollectionChanged += onDroneListChanged;

            foreach (IARDrone drone in _droneNetwork.Drones)
            {
                Drones.Add(new DroneViewModel(drone));
            }
        }

        public ObservableCollection<DroneViewModel> Drones { get; } = new ObservableCollection<DroneViewModel>();

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (_droneNetwork != null)
            {
                ((INotifyCollectionChanged)_droneNetwork.Drones).CollectionChanged -= onDroneListChanged;
                _droneNetwork.Dispose();
                _droneNetwork = null;
            }
        }

        private ARNetwork _droneNetwork;

        private void onDroneListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (e.OldItems != null)
                {
                    foreach (IARDrone drone in e.OldItems.Cast<IARDrone>())
                    {
                        DroneViewModel viewModel = Drones.FirstOrDefault(vm => vm.Model == drone);
                        if (viewModel != null)
                        {
                            Drones.Remove(viewModel);
                            viewModel.Dispose();
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (IARDrone drone in e.NewItems.Cast<IARDrone>())
                    {
                        Drones.Add(new DroneViewModel(drone));
                    }
                }
            });
        }
    }
}