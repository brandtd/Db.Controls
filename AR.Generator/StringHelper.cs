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
using System.Text;
using System.Text.RegularExpressions;

namespace AR.Generator
{
    /// <summary><c>string</c> extension methods useful during code generation.</summary>
    public static class StringHelper
    {
        /// <summary>Cleans up xml descriptions to remove excess whitespace and newline markers.</summary>
        public static string CleanUpXml(this string @this)
        {
            return Regex.Replace(@this.Replace("\\n", " ").Trim(), @"\s+", " ");
        }

        /// <summary>Convert a string to its camel case equivalent.</summary>
        public static string ToCamelCase(this string @this)
        {
            Regex regex = new Regex(@"[^A-Za-z0-9]");
            string intermediate = regex.Replace(@this, "_");
            string[] tokens = intermediate.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder builder = new StringBuilder(@this.Length);

            foreach (string token in tokens)
            {
                builder.Append(token.Substring(0, 1).ToUpper());

                if (token.Length > 1)
                {
                    builder.Append(token.Substring(1, token.Length - 1));
                }
            }

            return builder.ToString();
        }
    }
}