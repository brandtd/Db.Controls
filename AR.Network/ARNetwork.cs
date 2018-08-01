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
        /// <summary>
        ///     Create the network listener, but don't start it, allowing for events to be wired into
        ///     before the <see cref="StartSearching" /> method is called.
        /// </summary>
        public ARNetwork()
        {
        }

        /// <summary>Fired on discovering a new bebop device.</summary>
        public event EventHandler<ServiceDiscoveredArgs> BebopDiscovered;

        /// <summary>Fired on a bebop device leaving the network.</summary>
        public event EventHandler<ServiceLostArgs> BebopLost;

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (_listener != null)
            {
                _listener.ServiceFound -= onBebopFound;
                _listener.ServiceLost -= onBebopLost;
                _listener.Error -= onServiceError;
                _listener.Dispose();
                _listener = null;
            }
        }

        /// <summary>Start searching for drones.</summary>
        public void StartSearching()
        {
            _listener = ZeroconfResolver.CreateListener(_serviceName);
            _listener.ServiceFound += onBebopFound;
            _listener.ServiceLost += onBebopLost;
            _listener.Error += onServiceError;

            BebopDiscovered?.Invoke(this, new ServiceDiscoveredArgs(IPAddress.Parse("192.168.42.1"), 44444));
        }

        /// <summary>Event type for a new network service getting discovered.</summary>
        public class ServiceDiscoveredArgs : EventArgs
        {
            public ServiceDiscoveredArgs(IPAddress address, ushort port)
            {
                Address = address;
                Port = port;
            }

            /// <summary>Service's address.</summary>
            public IPAddress Address { get; }

            /// <summary>Service's port.</summary>
            public ushort Port { get; }
        }

        /// <summary>Event type for losing a network service.</summary>
        public class ServiceLostArgs : EventArgs
        {
            public ServiceLostArgs(IPAddress address, ushort port)
            {
                Address = address;
                Port = port;
            }

            /// <summary>Service's address.</summary>
            public IPAddress Address { get; }

            /// <summary>Service's port.</summary>
            public ushort Port { get; }
        }

        private const string _serviceName = "_arsdk-0901._udp.local.";
        private ZeroconfResolver.ResolverListener _listener;

        private void onBebopFound(object sender, IZeroconfHost host)
        {
            Console.WriteLine($"Discovered Bebop: {host.IPAddress}");

            IPAddress address = IPAddress.Parse(host.IPAddress);
            int port = host.Services[_serviceName].Port;

            BebopDiscovered?.Invoke(this, new ServiceDiscoveredArgs(address, (ushort)port));
        }

        private void onBebopLost(object sender, IZeroconfHost host)
        {
            Console.WriteLine($"Lost Bebop: {host.IPAddress}");

            IPAddress address = IPAddress.Parse(host.IPAddress);
            int port = host.Services[_serviceName].Port;

            BebopLost?.Invoke(this, new ServiceLostArgs(address, (ushort)port));
        }

        private void onServiceError(object sender, Exception e)
        {
            Console.WriteLine($"Error in zeroconf: {e.Message}");
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }
}