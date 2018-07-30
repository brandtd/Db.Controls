using System;

namespace AR.Common
{
    public static class ARPacker
    {
        public static bool Append(byte[] buffer, ref int index, bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, char value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, byte value)
        {
            if (buffer.Length - index >= 1)
            {
                buffer[index++] = value;
                return true;
            }

            return false;
        }

        public static bool Append(byte[] buffer, ref int index, sbyte value)
        {
            if (buffer.Length - index >= 1)
            {
                buffer[index++] = (byte)value;
                return true;
            }

            return false;
        }

        public static bool Append(byte[] buffer, ref int index, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return append(buffer, ref index, bytes);
        }

        public static bool Append(byte[] buffer, ref int index, string value)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
            if (append(buffer, ref index, bytes))
            {
                if (buffer.Length - index >= 1)
                {
                    buffer[index] = 0;
                    index++;
                    return true;
                }
            }

            return false;
        }

        private static bool append(byte[] buffer, ref int index, byte[] bytes)
        {
            if (buffer.Length - index >= bytes.Length)
            {
                Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
                index += bytes.Length;
                return true;
            }

            return false;
        }
    }
}