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

using AR.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AR.Network
{
    /// <summary>ARSDK 3 network frame.</summary>
    public class ARFrame
    {
        /// <summary>Possible frame types.</summary>
        public enum DataType
        {
            /// <summary>ACK'ing previous frame.</summary>
            Ack = 1,

            /// <summary>Contains data.</summary>
            Data = 2,

            /// <summary>Contains data (lower priority).</summary>
            LowLatencyData = 3,

            /// <summary>Contains data requiring an ACK.</summary>
            DataWithAck = 4,
        }

        /// <summary>Frame data (minus header).</summary>
        public byte[] Data { get; set; }

        /// <summary>Sequence number within frame buffer.</summary>
        public byte SequenceNumber { get; set; }

        /// <summary>Size of frame, including header.</summary>
        public int Size => _headerSize + (Data != null ? Data.Length : 0);

        /// <summary>Target buffer for frame.</summary>
        public byte TargetBuffer { get; set; }

        /// <summary>Frame type.</summary>
        public DataType Type { get; set; }

        /// <summary>
        ///     Decodes a frame from the given buffer, starting at the given index, incrementing the
        ///     index to account for the consumed data.
        /// </summary>
        public static ARFrame Decode(byte[] buffer, ref int index)
        {
            if (buffer.Length - index >= _headerSize)
            {
                int frameSize = BitConverter.ToInt32(buffer, 3 + index);

                if (buffer.Length - index >= frameSize)
                {
                    ARFrame frame = new ARFrame
                    {
                        Type = (DataType)buffer[0 + index],
                        TargetBuffer = buffer[1 + index],
                        SequenceNumber = buffer[2 + index],
                        Data = buffer.Skip(index + _headerSize).Take(frameSize - _headerSize).ToArray(),
                    };

                    index += frameSize;
                    return frame;
                }
            }

            return null;
        }

        /// <summary>Next sequence number to use for given buffer number.</summary>
        public static byte NextSequenceNumber(int buffer)
        {
            if (!_currentSequenceNumber.ContainsKey(buffer))
            {
                _currentSequenceNumber[buffer] = 0;
            }
            _currentSequenceNumber[buffer] = (byte)(_currentSequenceNumber[buffer] + 1 & 0xFF);
            return _currentSequenceNumber[buffer];
        }

        /// <summary>Encode this frame to a byte array, ready for sending.</summary>
        public byte[] Encode()
        {
            byte[] buffer = new byte[Size];

            int index = 0;
            ARPacker.Append(buffer, ref index, (byte)Type);
            ARPacker.Append(buffer, ref index, TargetBuffer);
            ARPacker.Append(buffer, ref index, SequenceNumber);
            ARPacker.Append(buffer, ref index, Size);

            if (Data != null)
            {
                Buffer.BlockCopy(Data, 0, buffer, index, Data.Length);
            }

            return buffer;
        }

        private const int _headerSize = 1 + 1 + 4 + 1;
        private static Dictionary<int, byte> _currentSequenceNumber = new Dictionary<int, byte>();
    }
}