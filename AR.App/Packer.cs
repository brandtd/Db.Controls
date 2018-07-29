using System;

namespace ParrotControl
{
    public static class Packer
    {
        public static void Append(byte[] buffer, ref int index, bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, char value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        public static void Append(byte[] buffer, ref int index, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }
    }
}