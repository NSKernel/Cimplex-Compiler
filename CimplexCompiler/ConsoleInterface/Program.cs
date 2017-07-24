using System;
using System.Collections.Generic;
using System.IO;


namespace NanoStudio.Internel.Cimplex.Compiler.ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            string Year = "2016";
            string Version = "Internel 0.0.0.4";
            //////////////////////////////////////////////////////////////////////////////
            //
            //  usage: cl SourceFile... [ Options... ]
            //  Ops:
            //  -nl No Logo
            //  -ol Object File Only
            //  -of Path of the output file (SourcePath + SourceFileName + Ext in default)
            //  -help
            //
            //////////////////////////////////////////////////////////////////////////////
            Console.Title = "Nano Studio Cimplex Compiler";
            bool OL = false;
            bool NL = false;
            bool OF = false;
            string SourcePath = "";
            string OutputPath = "";

            if(!NL)
                Console.WriteLine("Cimplex Compiler [Version: Internel " + Version + "]\nCopyright (C) " + Year + " Nano Studio. All rights reserved.\n========================================================\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: cl SourceFile... [ Options... ]");
                Environment.Exit(0);
            }
            else
            {
                if (args[0] != "-help" && args[0] != "-test")
                {
                    SourcePath = args[0];
                    for (int i = 1; i < args.Length; i++)
                    {
                        if (args[i] == "-nl")
                            NL = true;
                        else if (args[i] == "-of")
                        {
                            if (i + 1 < args.Length)
                                OutputPath = args[i + 1];
                            else
                                Console.WriteLine("Error: C0002 Output path not found.");
                        }
                        else if (args[i] == "-ol")
                            OL = true;
                        else if (args[i] == "-help")
                        {
                            Console.WriteLine("Usage: cl SourceFile... [ Options... ]");
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    if (args[0] == "-help")
                    {
                        Console.WriteLine("Usage: cl SourceFile... [ Options... ].\n       Extention of the file name is necessary.");
                        Console.WriteLine("Options:\n-nl				Show no logo.");
                        Console.WriteLine("-of [ OutputFilePath... ]	Sepcific the output file path. \n				Only avaliable without -ol\n				Extention name is necessary.");
                        Console.WriteLine("-ol				Generate object files without linking.");
                    }
                    else
                    {
                        StreamReader str = new StreamReader(@"C:\a.txt");
                        string b = str.ReadToEnd();
                        str.Close();
                        Preprocessor pp = new Preprocessor(b);
                        b = pp.PpPreprocess();
                        Scanner pl = new Scanner(b);
                        List<Compiler.Lexeme> c = pl.ScScan();
                        //foreach (Compiler.Lexeme s in c)
                        //    Console.WriteLine(s.Type.ToString() + "     " + s.Value);
                        List<IntermediateCode> parsed;
                        Parser pa = new Parser(c);
                        parsed = pa.PaParse();

                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }
                }
            }
        }
    }
}
