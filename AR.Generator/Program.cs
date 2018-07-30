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
using System.IO;
using System.Xml.Serialization;
using CommandLine;
using System.Collections.Generic;
using System.Reflection;
using HandlebarsDotNet;
using AR.Generator.Models;

namespace AR.Generator
{
    internal class Program
    {
        public static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
        }

        public static void HandleErrors(IEnumerable<Error> errors)
        {
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ProgramOptions>(args)
                .WithParsed<ProgramOptions>(opts => Run(opts))
                .WithNotParsed<ProgramOptions>(errors => HandleErrors(errors));
        }

        public static void Run(ProgramOptions options)
        {
            if (!Directory.Exists(options.OutputDirectory))
            {
                Directory.CreateDirectory(options.OutputDirectory);
            }
            Directory.Delete(options.OutputDirectory, true);

            string templateFileName = FormatResourceName(Assembly.GetExecutingAssembly(), "Templates/Command.template");
            using (StreamReader templateStream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(templateFileName)))
            {
                Action<TextWriter, object> templateAction = Handlebars.Compile(templateStream);

                foreach (string inputFileName in options.InputFiles)
                {
                    Console.WriteLine($"Parsing input file: {inputFileName}");

                    using (StreamReader inputFileReader = File.OpenText(inputFileName))
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(XmlProject));
                        if (xml.Deserialize(inputFileReader) is XmlProject projectDef)
                        {
                            foreach (XmlClass classDef in projectDef.Classes)
                            {
                                string classDirectory = $"{options.OutputDirectory}/{projectDef.Name.ToCamelCase()}.{classDef.Name.ToCamelCase()}";
                                if (!Directory.Exists(classDirectory))
                                {
                                    Directory.CreateDirectory(classDirectory);
                                }

                                foreach (XmlCommand commandDef in classDef.Commands)
                                {
                                    string outputFileName = $"{classDirectory}/{projectDef.Name.ToCamelCase()}.{classDef.Name.ToCamelCase()}.{commandDef.Name.ToCamelCase()}.cs";
                                    Console.WriteLine($"  Generating file: {outputFileName}");

                                    // Do this double write/read thing to ensure line endings are consistent.
                                    string tmpName = Path.GetTempFileName();
                                    using (StreamWriter tempWriter = new StreamWriter(tmpName))
                                    {
                                        CommandModel model = new CommandModel(projectDef, classDef, commandDef);
                                        templateAction(tempWriter, model);
                                    }

                                    using (StreamReader tempReader = new StreamReader(tmpName))
                                    {
                                        using (StreamWriter outputWriter = new StreamWriter(outputFileName))
                                        {
                                            while (!tempReader.EndOfStream)
                                            {
                                                outputWriter.WriteLine(tempReader.ReadLine());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}