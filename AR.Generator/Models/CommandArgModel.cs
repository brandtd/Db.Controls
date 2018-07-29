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

using AR.Commands.XmlSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.Generator.Models
{
    public class CommandArgModel
    {
        public CommandArgModel(XmlArg arg)
        {
            CamelCaseName = arg.Name.ToCamelCase();
            Summary = arg.Description.CleanUpXml();
            switch (arg.Type)
            {
                case XmlArgType.Byte: ClassType = "byte"; break;
                case XmlArgType.SByte: ClassType = "sbyte"; break;
                case XmlArgType.UShort: ClassType = "ushort"; break;
                case XmlArgType.Short: ClassType = "short"; break;
                case XmlArgType.UInt: ClassType = "uint"; break;
                case XmlArgType.Int: ClassType = "int"; break;
                case XmlArgType.Enum: ClassType = arg.Name.ToCamelCase() + "Enum"; break;
                case XmlArgType.ULong: ClassType = "ulong"; break;
                case XmlArgType.Long: ClassType = "long"; break;
                case XmlArgType.Float: ClassType = "float"; break;
                case XmlArgType.Double: ClassType = "double"; break;
                case XmlArgType.String: ClassType = "string"; break;
            }

            if (arg.Type == XmlArgType.Enum)
            {
                PackClassType = "int";
            }
            else
            {
                PackClassType = ClassType;
            }

            switch (arg.Type)
            {
                case XmlArgType.Byte:
                case XmlArgType.SByte:
                    Size = "1";
                    ConsumedBytes = Size;
                    break;

                case XmlArgType.UShort:
                case XmlArgType.Short:
                    Size = "2";
                    ConsumedBytes = Size;
                    break;

                case XmlArgType.UInt:
                case XmlArgType.Int:
                case XmlArgType.Float:
                case XmlArgType.Enum:
                    Size = "4";
                    ConsumedBytes = Size;
                    break;

                case XmlArgType.ULong:
                case XmlArgType.Long:
                case XmlArgType.Double:
                    Size = "8";
                    ConsumedBytes = Size;
                    break;

                case XmlArgType.String:
                    Size = $"({arg.Name.ToCamelCase()} != null ? {arg.Name.ToCamelCase()}.Length : 0)";
                    ConsumedBytes = Size + " + 1";
                    break;
            }

            switch (arg.Type)
            {
                case XmlArgType.Byte:
                case XmlArgType.SByte:
                case XmlArgType.String:
                    UsesBitConverter = false;
                    break;

                case XmlArgType.UShort:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToUInt16";
                    break;

                case XmlArgType.Short:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToInt16";
                    break;

                case XmlArgType.UInt:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToUInt32";
                    break;

                case XmlArgType.Int:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToInt32";
                    break;

                case XmlArgType.ULong:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToUInt64";
                    break;

                case XmlArgType.Long:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToInt64";
                    break;

                case XmlArgType.Float:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToSingle";
                    break;

                case XmlArgType.Double:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToDouble";
                    break;

                case XmlArgType.Enum:
                    UsesBitConverter = true;
                    BitConverterMethod = "ToInt32";
                    break;
            }

            IsString = arg.Type == XmlArgType.String;
        }

        public string BitConverterMethod { get; }
        public string CamelCaseName { get; }
        public string ClassType { get; }
        public string ConsumedBytes { get; }
        public bool IsString { get; }
        public string PackClassType { get; }
        public string Size { get; }
        public string Summary { get; }
        public bool UsesBitConverter { get; }
    }
}