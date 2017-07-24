using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoStudio.Internel.Cimplex.Compiler.ConsoleInterface
{
    class Error
    {
        public string Title = "";
        public string Details = "";
        public void Output()
        {
            Console.WriteLine("\nConsole Command Error.\nError: " + Title + "\nDetails: " + Details + "\n");
        }
    }
}
