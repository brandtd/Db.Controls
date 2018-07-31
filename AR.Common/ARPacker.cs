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