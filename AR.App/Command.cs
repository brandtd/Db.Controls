using System;
using System.Linq;

namespace ParrotControl
{
    public class Command
    {
        public byte ClassId { get; set; }
        public ushort CommandId { get; set; }
        public byte[] Data { get; set; }
        public byte FeatureId { get; set; }
        public int Size => _headerSize + (Data != null ? Data.Length : 0);

        public static Command Decode(byte[] buffer, int offset)
        {
            if (buffer.Length - offset >= _headerSize)
            {
                Command command = new Command
                {
                    FeatureId = buffer[0 + offset],
                    ClassId = buffer[0 + offset],
                    CommandId = buffer[0 + offset],
                    Data = buffer.Skip(offset + _headerSize).ToArray(),
                };

                return command;
            }

            return null;
        }

        public byte[] Encode()
        {
            byte[] buffer = new byte[Size];

            int index = 0;
            Packer.Append(buffer, ref index, FeatureId);
            Packer.Append(buffer, ref index, ClassId);
            Packer.Append(buffer, ref index, CommandId);

            if (Data != null)
            {
                Buffer.BlockCopy(Data, 0, buffer, index, Data.Length);
            }

            return buffer;
        }

        private const int _headerSize = 4;
    }
}