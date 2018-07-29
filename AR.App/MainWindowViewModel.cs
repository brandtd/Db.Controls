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

using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Zeroconf;

namespace ParrotControl
{
    public class MainWindowViewModel : PropertyChangedBase, IDisposable, INotifyPropertyChanged, IObserver<IZeroconfHost>
    {
        public MainWindowViewModel()
        {
            _subscription = ZeroconfResolver.ResolveContinuous(_serviceName).Subscribe(this);
            DroneDiscovery.Connect(IPAddress.Parse("192.168.42.1"), 44444).ContinueWith(t =>
            {
                Console.WriteLine(t.Result.Error);
            });
        }

        public string DisplayString
        {
            get => _displayString;
            set => SetProperty(ref _displayString, value);
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(IZeroconfHost value)
        {
            DisplayString = "Found drone, connecting ...";

            var address = value.IPAddress;
            var port = value.Services[_serviceName].Port;
            var addr = IPAddress.Parse(address);
            IConnectResult result = await DroneDiscovery.Connect(addr, (ushort)port);
            if (result.Drone != null)
            {
                DisplayString = "Connected to drone";
                result.Drone.Dispose();
            }
            else
            {
                DisplayString = "Failed to connect with drone:\n" + result.Error;
            }
        }

        private const string _serviceName = "_arsdk-0901._udp.local.";
        private string _displayString = "Searching ...";
        private IDisposable _subscription;
    }
}