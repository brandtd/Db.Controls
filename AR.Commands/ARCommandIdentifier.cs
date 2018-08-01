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

namespace AR.Commands
{
    /// <summary>Identifies an ARCommand type.</summary>
    public class ARCommandIdentifier : IEquatable<ARCommandIdentifier>
    {
        /// <summary>Create new identifier.</summary>
        public ARCommandIdentifier(byte featureId, byte classId, ushort commandId)
        {
            FeatureId = featureId;
            ClassId = classId;
            CommandId = commandId;
        }

        /// <summary>Class ID field, identifying command set within feature.</summary>
        public byte ClassId { get; }

        /// <summary>Command ID field, identifying command within class set.</summary>
        public ushort CommandId { get; }

        /// <summary>Feature/Project ID field, identifying possible command sets.</summary>
        public byte FeatureId { get; }

        /// <summary>Size of a command identifier.</summary>
        public int Size => _identifierSize;

        /// <summary>
        ///     Decode command identifier from given buffer, starting at given index, incrementing
        ///     index to account for consumed data.
        /// </summary>
        public static ARCommandIdentifier Decode(byte[] buffer, ref int index)
        {
            if (buffer.Length - index >= _identifierSize)
            {
                byte featureId = buffer[index];
                index += 1;

                byte classId = buffer[index];
                index += 1;

                ushort commandId = BitConverter.ToUInt16(buffer, index);
                index += 2;

                return new ARCommandIdentifier(featureId, classId, commandId);
            }

            return null;
        }

        /// <summary>
        ///     Encode command identifier into given buffer, starting at given index, incrementing
        ///     index to account for consumed data.
        /// </summary>
        /// <returns><c>true</c> if id encoded successfully, <c>false</c> otherwise.</returns>
        public bool Encode(byte[] buffer, ref int index)
        {
            bool success = true;
            int originalIndex = index;

            if (buffer.Length - index >= _identifierSize)
            {
                success &= ARPacker.Append(buffer, ref index, FeatureId);
                success &= ARPacker.Append(buffer, ref index, ClassId);
                success &= ARPacker.Append(buffer, ref index, CommandId);
            }

            if (!success)
            {
                index = originalIndex;
            }
            return success;
        }

        /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
        public bool Equals(ARCommandIdentifier other)
        {
            if (other != null)
            {
                return ClassId == other.ClassId &&
                       CommandId == other.CommandId &&
                       FeatureId == other.FeatureId;
            }

            return false;
        }

        /// <inheritdoc cref="object.Equals" />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }
            return Equals(obj as ARCommandIdentifier);
        }

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            int hashCode = 610168241;
            hashCode = hashCode * -1521134295 + ClassId.GetHashCode();
            hashCode = hashCode * -1521134295 + CommandId.GetHashCode();
            hashCode = hashCode * -1521134295 + FeatureId.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return $"{FeatureId}:{ClassId}:{CommandId}";
        }

        private const int _identifierSize = 4;
    }
}