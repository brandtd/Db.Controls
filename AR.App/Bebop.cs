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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParrotControl
{
    public class Bebop : IDisposable
    {
        public Bebop(UdpClient client)
        {
            _client = client;
            _cts = new CancellationTokenSource();
            _receiveTask = receiveContinuousAsync(_cts.Token);
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            _cts.Cancel();

            _client?.Dispose();
            _client = null;
        }

        private UdpClient _client;
        private CancellationTokenSource _cts;
        private Task _receiveTask;

        private async Task receiveContinuousAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result = await _client.ReceiveAsync();

                    int consumed = 0;
                    while (consumed < result.Buffer.Length)
                    {
                        Frame frame = Frame.Decode(result.Buffer, consumed, out int bytesRead);
                        consumed += bytesRead;

                        if (frame != null)
                        {
                            if (frame.TargetBuffer == 0)
                            {
                                byte[] response = new Frame
                                {
                                    Type = Frame.DataType.Data,
                                    TargetBuffer = 1,
                                    SequenceNumber = Frame.NextSequenceNumber(1),
                                    Data = frame.Data,
                                }.Encode();

                                await _client.SendAsync(response, response.Length, result.RemoteEndPoint);

                                Console.WriteLine("Bounced back packet");
                            }

                            if (frame.Type == Frame.DataType.DataWithAck)
                            {
                                byte[] response = new Frame
                                {
                                    Type = Frame.DataType.Ack,
                                    SequenceNumber = Frame.NextSequenceNumber(0x80 | frame.TargetBuffer),
                                    TargetBuffer = (byte)(0x80 | frame.TargetBuffer),
                                    Data = new byte[] { frame.SequenceNumber },
                                }.Encode();

                                await _client.SendAsync(response, response.Length, result.RemoteEndPoint);
                            }
                        }
                        else
                        {
                            consumed = result.Buffer.Length;
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}