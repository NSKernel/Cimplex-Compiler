using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NanoStudio.Internel.Cimplex.Compiler;

namespace NanoStudio.Internel.Cimplex.Compiler.ConsoleInterface
{
    class CompileAction
    {
        public static void CaCompile(string SourcePath, bool OL, string OutputPath)
        {
            // Read the code
            StreamReader sr = new StreamReader(SourcePath);
            string Code = sr.ReadToEnd();
            sr.Close();

            // Preprocess the code

        }
    }
}
