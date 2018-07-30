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
using System.Collections.Generic;

namespace AR.Generator.Models
{
    /// <summary>Creates a model of a command suitable for templating.</summary>
    public class CommandModel
    {
        /// <summary>
        ///     Create new command model for command within given project/feature and class.
        /// </summary>
        public CommandModel(XmlProject xmlProject, XmlClass xmlClass, XmlCommand xmlCommand)
        {
            ClassId = xmlClass.Id;
            ClassName = $"Cmd{xmlCommand.Name.ToCamelCase()}";
            CommandId = xmlCommand.Id;
            FeatureId = xmlProject.Id;
            Namespace = "ARParrot.Commands" + $".{xmlProject.Name.ToCamelCase()}.{xmlClass.Name.ToCamelCase()}";
            Summary = xmlCommand.Comment?.Description?.CleanUpXml() ?? "";

            string size = "0";
            Args = new List<CommandArgModel>();
            if (xmlCommand.Args != null)
            {
                foreach (XmlArg arg in xmlCommand.Args)
                {
                    CommandArgModel model = new CommandArgModel(arg);
                    Args.Add(model);

                    if (arg.Type == XmlArgType.Enum)
                    {
                        Enums.Add(new CommandEnumModel(arg));
                    }

                    size += $" + {model.Size}";
                }
            }
            Size = size;
        }

        public List<CommandArgModel> Args { get; }
        public int ClassId { get; }
        public string ClassName { get; }
        public int CommandId { get; }
        public List<CommandEnumModel> Enums { get; } = new List<CommandEnumModel>();
        public int FeatureId { get; }
        public bool HasEnums => Enums != null && Enums.Count > 0;

        public string LicenseBlock { get; } =
@"#region MIT License (c) 2018 Dan Brandt

// Copyright 2018 Dan Brandt

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the 'Software'), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion MIT License (c) 2018 Dan Brandt";

        public string Namespace { get; }
        public string Size { get; }
        public string Summary { get; }

        public string UsingBlock { get; } =
        @"using AR.Commands;
using AR.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;";
    }
}