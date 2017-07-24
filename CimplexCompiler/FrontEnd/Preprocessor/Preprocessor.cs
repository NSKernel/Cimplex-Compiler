/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1407141605
 Date of Generating: 2014/07/14
 File Name: Preprocessor.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Preprocessor Main.
  
 ==========================================================
 History:
 - 2014/07/14 16:05 : File created. Zhao Shixuan
***********************************************************/

using System.Collections.Generic;

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Preprocessor
    {
        private string PpOriginalCode = "";
        private int PpPositionNow = 0;
        private int PpPositionLast = 0;
        private bool PpIfStop = false;
        private int PpLineCount = 0;

        /// <summary>
        /// PreDefined Symbols
        /// </summary>
        public List<PreDefinedSymbol> PpDefinedSymbolTable = new List<PreDefinedSymbol>();

        /// <summary>
        /// The Cimplex Language Preprocessor Object Construtor
        /// </summary>
        /// <param name="Code">Original Code</param>
        public Preprocessor(string Code)
        {
            PpOriginalCode = Code;
        }

        /// <summary>
        /// Preprocess the original code.
        /// </summary>
        /// <returns>Processed code.</returns>
        public string PpPreprocess()
        {
            return PpPreprocessW();
        }

        /// <summary>
        /// Preprocessor worker method.
        /// </summary>
        /// <returns>Processed code.</returns>
        private string PpPreprocessW()
        {
            for(; PpPositionNow < PpOriginalCode.Length && (!PpIfStop); PpPositionNow++)
            {
                if (PpOriginalCode[PpPositionNow] == '"')
                {
                    PpPositionNow += 1;
                    if (PpPositionNow - 2 < 0)
                        PpIfStop = !PpInString(true);   // The stuff inside: show if it should escape or not
                    else
                        PpIfStop = !PpInString(PpOriginalCode[PpPositionNow - 2] != '@');   // The stuff inside: show if it should escape or not
                }
                else if (PpOriginalCode[PpPositionNow] == '\'')
                {
                    PpIfStop = !PpInChar();
                }
                else if (PpOriginalCode[PpPositionNow] == '\n')
                    PpLineCount += 1;
                else if (PpOriginalCode[PpPositionNow] == '/' && PpOriginalCode[PpPositionNow + 1] == '/')
                {
                    PpPositionLast = PpPositionNow;
                    PpPositionNow += 2;
                    PpCleanCPPComment();
                }
                else if (PpOriginalCode[PpPositionNow] == '/' && PpOriginalCode[PpPositionNow + 1] == '*')
                {
                    PpPositionLast = PpPositionNow;
                    PpPositionNow += 2;
                    PpCleanCComment();
                }
                else if(PpOriginalCode[PpPositionNow] == '#')
                {
                    PpProcessCommand();
                }
                else if (PpOriginalCode[PpPositionNow] == '*' && PpOriginalCode[PpPositionNow + 1] == '/')
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0001;
                    Error.ErrorDetail = "The \"/*\" is not found. Unexpected sign \"*/\".";
                    Error.LineNo = PpLineCount;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PpIfStop = true;
                }
            }
            if(PpIfStop)
            {
                PpOriginalCode = "";
            }

            return PpOriginalCode;
        } // PpPreprocessW : string

        /// <summary>
        /// Clean // like comment.
        /// </summary>
        private void PpCleanCPPComment()
        {
            for (; PpPositionNow < PpOriginalCode.Length; PpPositionNow++)
            {
                if (PpOriginalCode[PpPositionNow] == '\n')
                {
                    PpOriginalCode = PpOriginalCode.Substring(0, PpPositionLast) + "\n" + PpOriginalCode.Substring(PpPositionNow + 1, PpOriginalCode.Length - PpPositionNow - 1);
                    ///////////////////////////////////////////////////////////  A '\n' can show the right line status.
                    PpPositionNow = PpPositionLast; // then the "for" in PpPreprocessor will add 1 to the PpPositionNow
                    PpLineCount += 1;
                    break;
                }
                else if (PpPositionNow == PpOriginalCode.Length - 1)
                {
                    PpOriginalCode = PpOriginalCode.Substring(0, PpPositionLast) + "\n";
                    PpPositionNow = PpPositionLast;
                    break;
                }
            }
        } // PpCleanCPPComment : void

        /// <summary>
        /// Clean /**/ like comment.
        /// </summary>
        private void PpCleanCComment()
        {
            int LineCount = 0;
            for (; PpPositionNow < PpOriginalCode.Length; PpPositionNow++)
            {
                if (PpOriginalCode[PpPositionNow] == '*' && PpOriginalCode[PpPositionNow + 1] == '/')
                {
                    string LineChanger = " "; // = = A space is required.
                    for (int i = 0; i < LineCount; i++)
                    {
                        LineChanger += "\n";
                    }
                    PpOriginalCode = PpOriginalCode.Substring(0, PpPositionLast) + LineChanger + PpOriginalCode.Substring(PpPositionNow + 2, PpOriginalCode.Length - PpPositionNow - 2);
                    // The PpPositionLast should points to the start mark '/',
                    PpPositionNow = PpPositionLast + LineCount; // then the "for" in PpPreprocessor will add 1 to the PpPositionNow
                    break;
                }
                else if (PpOriginalCode[PpPositionNow] == '\n')
                {
                    LineCount += 1;
                    PpLineCount += 1;
                }
            }
            if(PpPositionNow == PpOriginalCode.Length)
            {
                Error Error = new Error();
                Error.ID = ErrorID.C0001;
                Error.ErrorDetail = "The \"/*\" is not found. Unexpected sign \"*/\".";
                Error.LineNo = PpLineCount;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PpIfStop = true;
            }
            
        } // PpCleanCComment : void

        private bool PpInString(bool IfEscape)
        {
            for (; PpPositionNow < PpOriginalCode.Length && PpOriginalCode[PpPositionNow] != '\"'; PpPositionNow++)
            {
                if (IfEscape == true)
                {
                    if (PpOriginalCode[PpPositionNow] == '\n')
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0002;
                        Error.ErrorDetail = "The character \"\\n\" is not allowed here.";
                        Error.LineNo = PpLineCount;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PpIfStop = true;
                        return false;
                    }
                    if (PpOriginalCode[PpPositionNow] == '\'')
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0002;
                        Error.ErrorDetail = "The character \"\'\" is not allowed here.";
                        Error.LineNo = PpLineCount;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PpIfStop = true;
                        return false;
                    }
                    if (PpOriginalCode[PpPositionNow] == '\\')
                    {
                        PpPositionNow += 1;
                        if (PpOriginalCode[PpPositionNow] != 'a' && PpOriginalCode[PpPositionNow] != 'b' &&
                            PpOriginalCode[PpPositionNow] != 'f' && PpOriginalCode[PpPositionNow] != 'n' &&
                            PpOriginalCode[PpPositionNow] != 'r' && PpOriginalCode[PpPositionNow] != 't' &&
                            PpOriginalCode[PpPositionNow] != 'v' && PpOriginalCode[PpPositionNow] != '0' &&
                            PpOriginalCode[PpPositionNow] != '\\' && PpOriginalCode[PpPositionNow] != '\'' &&
                            PpOriginalCode[PpPositionNow] != '\"')
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0003;
                            Error.ErrorDetail = "The character \"" + PpOriginalCode[PpPositionNow] + "\" is not a valid escape char.";
                            Error.LineNo = PpLineCount;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PpIfStop = true;
                            return false;
                        }
                    }
                }
            } // for
            if (PpPositionNow == PpOriginalCode.Length)
                return false;
            else
                return true;
        } // PpInString : bool

        private bool PpInChar()
        {
            if (PpOriginalCode[PpPositionNow + 1] != '\\')
            {
                if (PpPositionNow + 2 >= PpOriginalCode.Length || PpOriginalCode[PpPositionNow + 2] != '\'')
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0004;
                    Error.ErrorDetail = "The \"char\" type consists of one character.";
                    Error.LineNo = PpLineCount;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PpIfStop = true;
                    return false;
                }
                else if (PpOriginalCode[PpPositionNow + 1] == '\n')
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0002;
                    Error.ErrorDetail = "The character \"\\n\" is not allowed here.";
                    Error.LineNo = PpLineCount;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PpIfStop = true;
                    return false;
                }
                else if (PpOriginalCode[PpPositionNow + 1] == '\"')
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0002;
                    Error.ErrorDetail = "The character \"\'\" is not allowed here.";
                    Error.LineNo = PpLineCount;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PpIfStop = true;
                    return false;
                }
                else
                {
                    PpPositionNow += 2; // the "for" outside will add 1 in the PsParseStatement
                    return true;
                }
            }
            else
            {
                PpPositionNow += 2;
                if (PpOriginalCode[PpPositionNow] != 'a' && PpOriginalCode[PpPositionNow] != 'b' &&
                     PpOriginalCode[PpPositionNow] != 'f' && PpOriginalCode[PpPositionNow] != 'n' &&
                     PpOriginalCode[PpPositionNow] != 'r' && PpOriginalCode[PpPositionNow] != 't' &&
                     PpOriginalCode[PpPositionNow] != 'v' && PpOriginalCode[PpPositionNow] != '0' &&
                     PpOriginalCode[PpPositionNow] != '\\' && PpOriginalCode[PpPositionNow] != '\'' &&
                     PpOriginalCode[PpPositionNow] != '\"')
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0003;
                    Error.ErrorDetail = "The character \"" + PpOriginalCode[PpPositionNow] + "\" is not a valid escape char.";
                    Error.LineNo = PpLineCount;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PpIfStop = true;
                    return false;
                }
                if (PpOriginalCode[PpPositionNow + 1] != '\'')
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0004;
                    Error.ErrorDetail = "The \"char\" type consists of one character.";
                    Error.LineNo = PpLineCount;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PpIfStop = true;
                    return false;
                }
                else
                {
                    PpPositionNow += 1; // the "for" outside will add 1 in the PpPreprocessor
                    return true;
                }
            }
        } // PpInChar : bool
    }
}
