using System;
using System.Collections.Generic;
using System.Linq;

namespace ParrotControl
{
    public class Frame
    {
        public enum DataType
        {
            Ack = 1,
            Data = 2,
            LowLatencyData = 3,
            DataWithAck = 4,
        }

        public byte[] Data { get; set; }
        public byte SequenceNumber { get; set; }
        public int Size => _headerSize + (Data != null ? Data.Length : 0);
        public byte TargetBuffer { get; set; }
        public DataType Type { get; set; }

        public static Frame Decode(byte[] buffer, int offset, out int consumed)
        {
            if (buffer.Length - offset >= _headerSize)
            {
                int frameSize = BitConverter.ToInt32(buffer, 3 + offset);

                if (buffer.Length - offset >= frameSize)
                {
                    Frame frame = new Frame
                    {
                        Type = (DataType)buffer[0 + offset],
                        TargetBuffer = buffer[1 + offset],
                        SequenceNumber = buffer[2 + offset],
                        Data = buffer.Skip(offset + _headerSize).Take(frameSize - _headerSize).ToArray(),
                    };

                    consumed = frameSize;
                    return frame;
                }
            }

            consumed = 0;
            return null;
        }

        public static byte NextSequenceNumber(int buffer)
        {
            if (!_currentSequenceNumber.ContainsKey(buffer))
            {
                _currentSequenceNumber[buffer] = 0;
            }
            _currentSequenceNumber[buffer] = (byte)(_currentSequenceNumber[buffer] + 1 & 0xFF);
            return _currentSequenceNumber[buffer];
        }

        public byte[] Encode()
        {
            byte[] buffer = new byte[Size];

            int index = 0;
            Packer.Append(buffer, ref index, (byte)Type);
            Packer.Append(buffer, ref index, TargetBuffer);
            Packer.Append(buffer, ref index, SequenceNumber);
            Packer.Append(buffer, ref index, Size);

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