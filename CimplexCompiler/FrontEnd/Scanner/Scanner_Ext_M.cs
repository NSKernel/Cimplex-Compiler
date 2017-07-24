/***********************************************************
 Nano Studio Source File
 Copyright (C) 2014 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1408230823
 Date of Generating: 2014/08/23
 File Name: Scanner_Ext_M.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Extern methods of Sanner.
  
 ==========================================================
 History:
 - 2014/08/17 08:23 : File created. Zhao Shixuan
 - 2014/10/04 20:45 : Performance enhanced. Zhao Shixuan
 - 2014/10/09 21:36 : New error handler. Zhao Shixuan
 - 2015/03/27 15:35 : Stopped regarding the bracket and 
                      square bracket as a single lexeme.
                      Zhao Shixuan
***********************************************************/

using System.Text.RegularExpressions;

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Scanner
    {
        /// <summary>
        /// Return the last lexemem before the symbol
        /// </summary>
        private Lexeme ScGetLast()
        {
            //if (ScOriginalCode.Substring(ScPositionLast, ScPositionNow - ScPositionLast) != "") 
            // now it can't be
            //// nope, it can be empty for some reason = =
            ////// It can't be empty! Determined in the code above! =_=
            {
                Lexeme TempLexeme = new Lexeme();
                string TempLexemeValue = ScOriginalCode.Substring(ScPositionLast, ScPositionNow - ScPositionLast);
                //////////////////////////////////////////////////
                //
                //  Why not ScPositionNow - ScPositionLast + 1?
                //  
                //  aaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaa
                //    ^                          ^
                //    ScPositionLast             ScPositionNow
                //    if +1, the space will be added to the value.
                //
                //////////////////////////////////////////////////
                TempLexeme.Value = TempLexemeValue;
                TempLexeme.LineNumber = ScLineCount;

                // Keywords
                //// Type
                if (TempLexemeValue == "new")
                {
                    TempLexeme.Type = LexemeType.LV2_OPERATOR;
                    TempLexeme.OperatorPriority = 2;
                    TempLexeme.Value = Operators.NEW;
                } else if (TempLexemeValue == "bool")
                    TempLexeme.Type = LexemeType.BOOL;
                ////// NUMBER
                else if (TempLexemeValue == "int")
                    TempLexeme.Type = LexemeType.INT;
                else if (TempLexemeValue == "float")
                    TempLexeme.Type = LexemeType.FLOAT;
                else if (TempLexemeValue == "double")
                    TempLexeme.Type = LexemeType.DOUBLE;

                else if (TempLexemeValue == "char")
                    TempLexeme.Type = LexemeType.CHAR;
                else if (TempLexemeValue == "string")
                    TempLexeme.Type = LexemeType.STRING;
                else if (TempLexemeValue == "void")
                    TempLexeme.Type = LexemeType.VOID;
                else if (TempLexemeValue == "class")
                    TempLexeme.Type = LexemeType.CLASS;
                else if (TempLexemeValue == "enum")
                    TempLexeme.Type = LexemeType.ENUM;
                //// Logic
                else if (TempLexemeValue == "if")
                    TempLexeme.Type = LexemeType.IF;
                else if (TempLexemeValue == "else")
                    TempLexeme.Type = LexemeType.ELSE;
                else if (TempLexemeValue == "do")
                    TempLexeme.Type = LexemeType.DO;
                else if (TempLexemeValue == "while")
                    TempLexeme.Type = LexemeType.WHILE;
                else if (TempLexemeValue == "for")
                    TempLexeme.Type = LexemeType.FOR;
                else if (TempLexemeValue == "break")
                    TempLexeme.Type = LexemeType.BREAK;
                else if (TempLexemeValue == "continue")
                    TempLexeme.Type = LexemeType.CONTINUE;
                else if (TempLexemeValue == "goto")
                    TempLexeme.Type = LexemeType.GOTO;


                else if (TempLexemeValue == "this")
                    TempLexeme.Type = LexemeType.THIS;

                //// Modifier
                else if (TempLexemeValue == "public")
                    TempLexeme.Type = LexemeType.PUBLIC;
                else if (TempLexemeValue == "private")
                    TempLexeme.Type = LexemeType.PRIVATE;
                else if (TempLexemeValue == "protected")
                    TempLexeme.Type = LexemeType.PROTECTED;
                else if (TempLexemeValue == "virtual")
                    TempLexeme.Type = LexemeType.VIRTUAL;
                else if (TempLexemeValue == "override")
                    TempLexeme.Type = LexemeType.OVERRIDE;
                else if (TempLexemeValue == "static")
                    TempLexeme.Type = LexemeType.STATIC;
                else if (TempLexemeValue == "const")
                    TempLexeme.Type = LexemeType.CONST;
                else if (TempLexemeValue == "final")
                    TempLexeme.Type = LexemeType.FINAL;
                //// Values
                else if (TempLexemeValue == "true")
                    TempLexeme.Type = LexemeType.TRUE;
                else if (TempLexemeValue == "false")
                    TempLexeme.Type = LexemeType.FALSE;
                else if (TempLexemeValue == "null")
                    TempLexeme.Type = LexemeType.NULL;

                else if (TempLexemeValue == "try")
                    TempLexeme.Type = LexemeType.TRY;
                else if (TempLexemeValue == "catch")
                    TempLexeme.Type = LexemeType.CATCH;
                else if (TempLexemeValue == "finally")
                    TempLexeme.Type = LexemeType.FINALLY;
                else if (TempLexemeValue == "throw")
                    TempLexeme.Type = LexemeType.THROW;

                else if (TempLexemeValue == "return")
                    TempLexeme.Type = LexemeType.RETURN;


                else if (Regex.Match(TempLexemeValue, @"^[A-Za-z_]([A-Za-z0-9_]*)$").Success)
                {
                    TempLexeme.Type = LexemeType.ID;
                }
                else if (Regex.Match(TempLexemeValue, "^([0-9]*)$").Success || Regex.Match(TempLexemeValue, @"^([0-9]*)(\.([0-9])*)?(E[+-]?([0-9]*))$").Success)
                {
                    TempLexeme.Type = LexemeType.INT_VALUE;
                    int Temp = 0;
                    int.TryParse(TempLexemeValue, out Temp);
                    TempLexeme.Value = Temp;
                }
                else if (Regex.Match(TempLexemeValue, @"^([0-9]*)\.([0-9]*)$").Success)
                {
                    TempLexeme.Type = LexemeType.FLOAT_VALUE;
                    double Temp = 0;
                    double.TryParse(TempLexemeValue, out Temp);
                    TempLexeme.Value = Temp;
                }
                else
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0009;
                    Error.ErrorDetail = "The scanner failed to recogonize the type of the lexeme \"" + TempLexemeValue + "\". Check your spelling.";
                    Error.LineNo = ScLineCount;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    ScStopScanning = true;
                }

                return TempLexeme;
            } // //if Bolck
        } // ScGetLast : Lexeme

        #region AreaProcessors

        /// <summary>
        /// Process the contents in Brace
        /// </summary>
        private void ScInBrace()
        {
            int BraceDepth = 1;                         // To show how many brace there are.
            for (; ScPositionNow < ScOriginalCode.Length && BraceDepth != 0; ScPositionNow++)
            {
                if (ScOriginalCode[ScPositionNow] == '"')
                    ScInString(ScOriginalCode[ScPositionNow - 1] != '@', false);
                else if (ScOriginalCode[ScPositionNow] == '\'')
                    ScInChar(false);
                else if (ScOriginalCode[ScPositionNow] == '{')
                    BraceDepth += 1;
                else if (ScOriginalCode[ScPositionNow] == '}')
                    BraceDepth -= 1;
                else if (ScOriginalCode[ScPositionNow] == '\n')
                    ScLineCount += 1;
            }
            ScPositionNow -= 1;
            if (BraceDepth != 0)
            {
                Error Error = new Error();
                Error.ID = ErrorID.C0008;
                Error.ErrorDetail = "A '}' must come after the contents in the bracket.";
                Error.LineNo = ScLineCount;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error); 
                ScStopScanning = true;
            }
            else
            {
                return;
            }
        } // ScInBrace : void

        /// <summary>
        /// Process string
        /// </summary>
        /// <param name="IfEscape">Determine if escape</param>
        /// <param name="IfReplace">Determine if replace the escape chars</param>
        private void ScInString(bool IfEscape, bool IfReplace)
        {
            ScPositionNow += 1;
            for (; ScPositionNow < ScOriginalCode.Length && ScOriginalCode[ScPositionNow] != '\"'; ScPositionNow++)
            {
                if (IfEscape == true)
                {
                    // if (ScOriginalCode[ScPositionNow] == '\n')
                    
                    // if (ScOriginalCode[ScPositionNow] == '\'')
                    
                    if (ScOriginalCode[ScPositionNow] == '\\')
                    {
                        ScPositionNow += 1;
                        // if (ScOriginalCode[ScPositionNow] != 'a' && ScOriginalCode[ScPositionNow] != 'b' &&
                        //    ScOriginalCode[ScPositionNow] != 'f' && ScOriginalCode[ScPositionNow] != 'n' &&
                        //    ScOriginalCode[ScPositionNow] != 'r' && ScOriginalCode[ScPositionNow] != 't' &&
                        //    ScOriginalCode[ScPositionNow] != 'v' && ScOriginalCode[ScPositionNow] != '0' &&
                        //    ScOriginalCode[ScPositionNow] != '\\' && ScOriginalCode[ScPositionNow] != '\'' &&
                        //    ScOriginalCode[ScPositionNow] != '\"')
                        if (IfReplace)
                            ScReplaceEscapeChar();
                        
                    }
                }
            } // for
            if (ScPositionNow == ScOriginalCode.Length + 1)
            {
                ScStopScanning = true;
                return;
            }
            else
            {
                return;
            }
        } // ScInString : void

        /// <summary>
        /// Process char
        /// </summary>
        /// <param name="IfReplace">Determine if replace the escape chars</param>
        private void ScInChar(bool IfReplace)
        {
            if (ScOriginalCode[ScPositionNow + 1] != '\\')
            {
                // if (ScPositionNow + 2 >= ScOriginalCode.Length || ScOriginalCode[ScPositionNow + 2] != '\'')
                
                // else if (ScOriginalCode[ScPositionNow + 1] == '\n')
                
                // else if (ScOriginalCode[ScPositionNow + 1] == '\"')
                
                ScPositionNow += 2;
                return;
            }
            else
            {
                ScPositionNow += 2;
                //if (ScOriginalCode[ScPositionNow] != 'a' && ScOriginalCode[ScPositionNow] != 'b' &&
                //     ScOriginalCode[ScPositionNow] != 'f' && ScOriginalCode[ScPositionNow] != 'n' &&
                //     ScOriginalCode[ScPositionNow] != 'r' && ScOriginalCode[ScPositionNow] != 't' &&
                //     ScOriginalCode[ScPositionNow] != 'v' && ScOriginalCode[ScPositionNow] != '0' &&
                //     ScOriginalCode[ScPositionNow] != '\\' && ScOriginalCode[ScPositionNow] != '\'' &&
                //     ScOriginalCode[ScPositionNow] != '\"')
                //if (ScOriginalCode[ScPositionNow + 1] != '\'')
                if (IfReplace)
                    ScReplaceEscapeChar();
                ScPositionNow += 1;
                return;
                
            }
        } // ScInChar : void

        /// <summary>
        /// Replace the escape char
        /// </summary>
        private void ScReplaceEscapeChar()
        {
            char temp = '\0';
            if (ScOriginalCode[ScPositionNow] == 'a')
            {
                temp = '\a';
            }
            else if (ScOriginalCode[ScPositionNow] == 'b')
            {
                temp = '\b';
            }
            else if (ScOriginalCode[ScPositionNow] == 'f')
            {
                temp = '\f';
            }
            else if (ScOriginalCode[ScPositionNow] == 'n')
            {
                temp = '\n';
            }
            else if (ScOriginalCode[ScPositionNow] == 'r')
            {
                temp = '\r';
            }
            else if (ScOriginalCode[ScPositionNow] == 't')
            {
                temp = '\t';
            }
            else if (ScOriginalCode[ScPositionNow] == 'v')
            {
                temp = '\v';
            }
            else if (ScOriginalCode[ScPositionNow] == '0')
            {
                temp = '\0';
            }
            else if (ScOriginalCode[ScPositionNow] == '\\')
            {
                temp = '\\';
            }
            else if (ScOriginalCode[ScPositionNow] == '\'')
            {
                temp = '\'';
            }
            else if (ScOriginalCode[ScPositionNow] == '\"')
            {
                temp = '\"';
            }
            // change the escape char!
            ScOriginalCode = ScOriginalCode.Substring(0, ScPositionNow - 1) + temp.ToString() + ScOriginalCode.Substring(ScPositionNow + 1, ScOriginalCode.Length - ScPositionNow - 1);
            ScPositionNow -= 1;
        } // ScReplaceEscapeChar : void

        #endregion AreaProcessors
    }
}
