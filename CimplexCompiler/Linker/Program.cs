using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace NanoStudio.Internel.Cimplex.Compiler.Linker.ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if(args.Length != 0)
            {

            }
            // Copyright info
            Console.WriteLine("Nano Studio EC Linker [Version: Internel 0.0.0.2]\nCopyright (C) 2014 Nano Studio. All rights reserved.\n========================================================\n");
            
            // 
            while (true)
            {
                string Command;
                Console.Write("$> ");
                Command = Console.ReadLine();
                string[] LookAHead = Command.Split(' ');
                int i = 0;
                for (; i < LookAHead.Length && LookAHead[i] == ""; i++) ;

                
                if (i < LookAHead.Length)
                {
                    if(LookAHead[i] == "")
                    {

                    }

                    if(LookAHead[i] == "testregex")
                    {
                        //string temp = Command.Substring(3, Command.Length - 3);
                        string temp = "0314";
                        if (Regex.Match(temp, @"^([0-9]*)(\.[0-9]*)$").Success)
                            Console.WriteLine("Yes");
                        else
                            Console.WriteLine("No");
                    }

                    if (LookAHead[i] == "testnum")
                    {
                        Console.WriteLine(2e2-5);
                        
                    }
                }
            }
        }

        private static string[] RequireParameter(int Count, string[] ParameterTitle)
        {
            string[] Finished = ParameterTitle;
            Console.WriteLine("\nRequiring following parameters");
            for(int i = 0; i < Count; i++)
            {
                Console.Write(ParameterTitle[i] + "> ");
                Finished[i] = Console.ReadLine();
            }
            Console.WriteLine("");
            return Finished;
        }
    }
}
