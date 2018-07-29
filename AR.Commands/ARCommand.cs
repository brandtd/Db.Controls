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

namespace AR.Commands
{
    /// <summary>Base class for an ARSDK command.</summary>
    public abstract class ARCommand
    {
        /// <summary>Size of command's data section (excludes command's header).</summary>
        public virtual int ARCommandSize => 0;

        /// <summary>Identifier for command.</summary>
        public ARCommandIdentifier Id { get; }

        /// <summary>
        ///     Decode command's data from a <c>byte[]</c>, starting at the given index, incrementing
        ///     index to account for consumed bytes.
        /// </summary>
        public abstract bool Decode(byte[] buffer, ref int index);

        /// <summary>
        ///     Encode command's data into the given buffer, starting at the given index and
        ///     incrementing the index as data is inserted into the buffer.
        /// </summary>
        public virtual bool Encode(byte[] buffer, ref int index)
        {
            return true;
        }

        /// <summary>Create new command with given IDs.</summary>
        protected ARCommand(ARCommandIdentifier id)
        {
            Id = id;
        }
    }
}