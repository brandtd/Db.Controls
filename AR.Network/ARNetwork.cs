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
using ARParrot.Commands.Common.Settings;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using Zeroconf;

namespace AR.Network
{
    public class ARNetwork : IDisposable
    {
        public ARNetwork()
        {
            _codec = new ARCommandCodec();
            _codec.RegisterCommands(Assembly.GetAssembly(typeof(ARCommand)));
            _codec.RegisterCommands(Assembly.GetAssembly(typeof(CmdAllSettings)));

            _listener = ZeroconfResolver.CreateListener(_serviceName);
            _listener.ServiceFound += onServiceFound;
            _listener.ServiceLost += onServiceLost;
            _listener.Error += onServiceError;

            /*ARBebop drone = new ARBebop(_codec);
            drone.Connect(IPAddress.Parse("192.168.42.1"), 44444).ContinueWith(
                t =>
                {
                    if (string.IsNullOrEmpty(t.Result))
                    {
                        Drones.Add(drone);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());*/
        }

        /// <summary>Discovered/connected drones.</summary>
        public ObservableCollection<IARDrone> Drones { get; } = new ObservableCollection<IARDrone>();

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (_listener != null)
            {
                _listener.ServiceFound -= onServiceFound;
                _listener.ServiceLost -= onServiceLost;
                _listener.Error -= onServiceError;
                _listener.Dispose();
                _listener = null;
            }
        }

        private const string _serviceName = "_arsdk-0901._udp.local.";
        private ARCommandCodec _codec;
        private ZeroconfResolver.ResolverListener _listener;

        private void onServiceError(object sender, Exception e)
        {
            Console.WriteLine($"Error in zeroconf: {e.Message}");
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private async void onServiceFound(object sender, IZeroconfHost host)
        {
            string address = host.IPAddress;
            int port = host.Services[_serviceName].Port;
            IPAddress addr = IPAddress.Parse(address);

            if (Drones.FirstOrDefault(d => d.Address == addr) == null)
            {
                Console.WriteLine($"Discovered service: {address}:{port}");

                ARBebop drone = new ARBebop(_codec);
                string error = await drone.Connect(addr, (ushort)port);

                if (string.IsNullOrEmpty(error))
                {
                    Drones.Add(drone);
                }
            }
        }

        private void onServiceLost(object sender, IZeroconfHost host)
        {
            Console.WriteLine($"Lost service: {host.IPAddress}");

            IPAddress hostAddress = IPAddress.Parse(host.IPAddress);
            IARDrone drone = Drones.FirstOrDefault(d => d.Address == hostAddress);
            if (drone != null)
            {
                Drones.Remove(drone);
                (drone as IDisposable)?.Dispose();
            }
        }
    }
}