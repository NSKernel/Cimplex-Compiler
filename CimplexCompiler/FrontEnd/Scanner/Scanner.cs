/***********************************************************
 Nano Studio Source File
 Copyright (C) 2014 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1407141024
 Date of Generating: 2014/07/14
 File Name: Scanner.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
 
 ==========================================================
 Abstract: Lexical analyzer.
  
 ==========================================================
 History:
 - 2014/07/14 10:24 : File created. Zhao Shixuan
 - 2014/08/08 18:10 : 0.0.0.2 Finished. Zhao Shixuan
 - 2014/09/07 17:34 : 0.0.0.3 Repaired the bug while 
                              processing float value. 
                              Zhao Shixuan
 - 2014/09/12 16:12 : 0.0.0.4 Added adding the Statement
                              End Mark after a block
                              Zhao Shixuan
 - 2014/10/07 13:16 : 0.0.0.5 Enhanced performance
                              Zhao Shixuan
 - 2014/10/09 21:36 : 0.0.0.6 New error handler.
                              Zhao Shixuan
 - 2015/03/27 15:35 : 0.0.0.7 Stopped regarding the bracket 
                              and square bracket as a single
                              lexeme.
                              Zhao Shixuan
 - 2016/11/01 15:10 : 0.0.0.8 Removed LINE_END_MARK
                              Zhao Shixuan
***********************************************************/

using System.Collections.Generic;                // List<T> is inside! ┌(ioi))┐)┐)┐)┐)┐)┐

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Scanner
    {
        /// <summary>
        /// The Cimplex Language Lexical Analyzer Object Constructor.
        /// </summary>
        /// <param name="Code">The code to be scanned.</param>
        public Scanner(string Code)
        {
            ScOriginalCode = Code;
        } // Scanner : Constructor

        #region GlobalVariables

        /// <summary>
        /// Original Code
        /// </summary>
        private string ScOriginalCode = "";

        /// <summary>
        /// Pointer to the position now in the string.
        /// </summary>
        private int ScPositionNow = 0;

        /// <summary>
        /// Pointer to the last point in the string
        /// </summary>
        private int ScPositionLast = 0;

        /// <summary>
        /// To stop scanning a piece of code
        /// </summary>
        private bool ScStopScanning = false;

        /// <summary>
        /// Line number. Use when displaying errors.
        /// </summary>
        private int ScLineCount = 1;

        #endregion GlobalVariables

        /// <summary>
        /// Scan the code of input.
        /// </summary>
        /// <returns>Analyzed lexeme stream.</returns>
        public List<Lexeme> ScScan()
        {
            return ScScanW();
        } // ScScan : List<Lexeme>

        /// <summary>
        /// Lexical analyzer worker method.
        /// </summary>
        /// <returns>Analyzed lexeme stream.</returns>
        private List<Lexeme> ScScanW()
        {            
            List<Lexeme> TempData = new List<Lexeme>();
            

            for (; ScPositionNow < ScOriginalCode.Length && !ScStopScanning; ScPositionNow++)
            {
                Lexeme TempLexeme = new Lexeme();
                TempLexeme.LineNumber = ScLineCount;
                switch (ScOriginalCode[ScPositionNow])
                {
                    case ' ':
                        if (ScPositionNow - ScPositionLast >= 1)
                        {
                            TempData.Add(ScGetLast());
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                            ScPositionLast++;
                        break;
                    case '\r':
                        if (ScPositionNow - ScPositionLast >= 1)
                        {
                            TempData.Add(ScGetLast());
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                            ScPositionLast++;
                        break;
                    case '\n':
                        if (ScPositionNow - ScPositionLast >= 1)
                        {
                            TempData.Add(ScGetLast());
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                            ScPositionLast++;
                        ScLineCount += 1;
                        break;

                    // List below:
                    // +, +=, ++
                    // -, -=, --
                    // *, *=
                    // /, /=
                    // %, %=

                    // +, +=, ++
                    case '+':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV9_OPERATOR;
                            TempLexeme.OperatorPriority = 9;
                            TempLexeme.Value = Operators.ADD_ASSIGN;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else if (ScOriginalCode[ScPositionNow + 1] == '+')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            if (TempData[TempData.Count - 1].Type == LexemeType.ID)
                            {
                                TempLexeme.Type = LexemeType.LV1_OPERATOR; // While being used as an LEXEME++
                                TempLexeme.OperatorPriority = 1;
                                TempLexeme.Value = Operators.INCREASE_POSTFIX;
                            }
                            else
                            {
                                TempLexeme.Type = LexemeType.LV2_OPERATOR;
                                TempLexeme.Value = Operators.INCREASE_PREFIX;
                            }
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            if (TempData.Count > 0
                                && (TempData[TempData.Count - 1].Type == LexemeType.ID
                                || TempData[TempData.Count - 1].Type == LexemeType.INT_VALUE
                                || TempData[TempData.Count - 1].Type == LexemeType.FLOAT_VALUE    // no Double_Value!
                                || TempData[TempData.Count - 1].Type == LexemeType.STRING_VALUE
                                || TempData[TempData.Count - 1].Type == LexemeType.RIGHT_BRACKET
                                || TempData[TempData.Count - 1].Type == LexemeType.RIGHT_SQUARE_BRACKET))
                            {
                                TempLexeme.Type = LexemeType.LV4_OPERATOR;
                                TempLexeme.OperatorPriority = 4;
                                TempLexeme.Value = Operators.ADDITION;
                            }
                            else
                            {
                                TempLexeme.Type = LexemeType.LV2_OPERATOR;
                                TempLexeme.OperatorPriority = 2;
                                TempLexeme.Value = Operators.POSITIVE;
                            }
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // -, -=, --
                    case '-':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV9_OPERATOR;
                            TempLexeme.OperatorPriority = 9;
                            TempLexeme.Value = Operators.SUB_ASSIGN;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else if (ScOriginalCode[ScPositionNow + 1] == '-')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            if (TempData[TempData.Count - 1].Type == LexemeType.ID)
                            {
                                TempLexeme.Type = LexemeType.LV1_OPERATOR; // While being used as an LEXEME--
                                TempLexeme.OperatorPriority = 1;
                                TempLexeme.Value = Operators.DECREASE_POSTFIX;
                            }
                            else
                            {
                                TempLexeme.Type = LexemeType.LV2_OPERATOR;
                                TempLexeme.OperatorPriority = 2;
                                TempLexeme.Value = Operators.DECREASE_PREFIX;
                            }
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            if (TempData.Count > 0
                                && (TempData[TempData.Count - 1].Type == LexemeType.ID
                                || TempData[TempData.Count - 1].Type == LexemeType.INT_VALUE
                                || TempData[TempData.Count - 1].Type == LexemeType.FLOAT_VALUE    // no Double_Value!
                                || TempData[TempData.Count - 1].Type == LexemeType.RIGHT_BRACKET
                                || TempData[TempData.Count - 1].Type == LexemeType.RIGHT_SQUARE_BRACKET)) // string is not supported!
                            {
                                TempLexeme.Type = LexemeType.LV4_OPERATOR;
                                TempLexeme.OperatorPriority = 4;
                                TempLexeme.Value = Operators.SUBTRACTION;
                            }
                            else
                            {
                                TempLexeme.Type = LexemeType.LV2_OPERATOR;
                                TempLexeme.OperatorPriority = 2;
                                TempLexeme.Value = Operators.NEGTIVE;
                            }
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // *, *=
                    case '*':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV9_OPERATOR;
                            TempLexeme.OperatorPriority = 9;
                            TempLexeme.Value = Operators.MUL_ASSIGN;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV3_OPERATOR;
                            TempLexeme.OperatorPriority = 3;
                            TempLexeme.Value = Operators.MULTIPLICATION;
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // /, /=
                    case '/':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV9_OPERATOR;
                            TempLexeme.OperatorPriority = 9;
                            TempLexeme.Value = Operators.DIV_ASSIGN;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV3_OPERATOR;
                            TempLexeme.OperatorPriority = 3;
                            TempLexeme.Value = Operators.DIVISION;
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // %, %=
                    case '%':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV9_OPERATOR;
                            TempLexeme.OperatorPriority = 9;
                            TempLexeme.Value = Operators.MOD_ASSIGN;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV3_OPERATOR;
                            TempLexeme.OperatorPriority = 3;
                            TempLexeme.Value = Operators.MOD;
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // =
                    case '=':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV6_OPERATOR;
                            TempLexeme.OperatorPriority = 6;
                            TempLexeme.Value = Operators.EQUAL;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV9_OPERATOR;
                            TempLexeme.OperatorPriority = 9;
                            TempLexeme.Value = Operators.ASSIGN;
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // >, >=
                    case '>':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV5_OPERATOR;
                            TempLexeme.OperatorPriority = 5;
                            TempLexeme.Value = Operators.GREATER_OR_EQUAL;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV5_OPERATOR;
                            TempLexeme.OperatorPriority = 5;
                            TempLexeme.Value = Operators.GREATER_THAN;
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // <, <=
                    case '<':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV5_OPERATOR;
                            TempLexeme.OperatorPriority = 5;
                            TempLexeme.Value = Operators.LESS_OR_EQUAL;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV5_OPERATOR;
                            TempLexeme.OperatorPriority = 5;
                            TempLexeme.Value = Operators.LESS_THAN;
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // !
                    case '!':
                        if (ScOriginalCode[ScPositionNow + 1] == '=')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV6_OPERATOR;
                            TempLexeme.OperatorPriority = 6;
                            TempLexeme.Value = Operators.NOT_EQUAL;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV2_OPERATOR;
                            TempLexeme.OperatorPriority = 2;
                            TempLexeme.Value = Operators.OPPSITE;
                            TempData.Add(TempLexeme);
                            ScPositionLast = ScPositionNow + 1;
                        }
                        break;
                    // &
                    case '&':
                        if (ScOriginalCode[ScPositionNow + 1] == '&')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV7_OPERATOR;
                            TempLexeme.OperatorPriority = 7;
                            TempLexeme.Value = Operators.AND;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            ScStopScanning = true;
                            Error Error = new Error();
                            Error.ID = ErrorID.C0005;
                            Error.ErrorDetail = "Bitwise is not supported in this version of language.";
                            Error.LineNo = ScLineCount;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        }
                        break;
                    // |
                    case '|':
                        if (ScOriginalCode[ScPositionNow + 1] == '|')
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            TempLexeme.Type = LexemeType.LV8_OPERATOR;
                            TempLexeme.OperatorPriority = 8;
                            TempLexeme.Value = Operators.OR;
                            TempData.Add(TempLexeme);
                            ScPositionNow += 1;
                            ScPositionLast = ScPositionNow + 1;
                        }
                        else
                        {
                            ScStopScanning = true;
                            Error Error = new Error();
                            Error.ID = ErrorID.C0005;
                            Error.ErrorDetail = "Bitwise is not supported in this version of language.";
                            Error.LineNo = ScLineCount;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        }
                        break;
                    // .
                    case '.':
                        // Float value needs to be determined.
                        if (ScGetLast().Type == LexemeType.INT_VALUE && char.IsDigit(ScOriginalCode[ScPositionNow - 1]) && char.IsDigit(ScOriginalCode[ScPositionNow + 1]))
                        {
                            // Do nothing - double
                        }
                        else
                        {
                            if (ScPositionNow - ScPositionLast >= 1)
                                TempData.Add(ScGetLast());
                            ScPositionLast = ScPositionNow + 1;
                            TempLexeme.Type = LexemeType.LV1_OPERATOR;
                            TempLexeme.OperatorPriority = 1;
                            TempLexeme.Value = Operators.GET_CHILD;
                            TempData.Add(TempLexeme);
                        }
                        break;
                    // ,
                    case ',':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        ScPositionLast = ScPositionNow + 1;
                        TempLexeme.Type = LexemeType.LV10_OPERATOR;
                        TempLexeme.OperatorPriority = 10;
                        TempLexeme.Value = Operators.COMMA;
                        TempData.Add(TempLexeme);
                        break;
                    // '
                    case '\'':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        ScPositionLast = ScPositionNow;
                        ScInChar(true);
                        TempLexeme.Type = LexemeType.CHAR_VALUE;
                        TempLexeme.Value = ScOriginalCode.Substring(ScPositionLast + 1, ScPositionNow - ScPositionLast - 1);
                        TempData.Add(TempLexeme);
                        ScPositionLast = ScPositionNow + 1;
                        break;
                    // @
                    case '@':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        if (ScOriginalCode[ScPositionNow + 1] != '"')
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0006;
                            Error.ErrorDetail = "Symbol '@' must come before a string without any character.";
                            Error.LineNo = ScLineCount;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            ScStopScanning = true;
                        }
                        ///////////////////////////////////////////////////////////////////
                        //
                        //  Why commented the following code
                        //  The escaped chars have been replaced, so the @ becomes useless
                        //  and can be ignored.
                        //
                        ///////////////////////////////////////////////////////////////////
                        //
                        //  TempLexeme.Type = LexemeType.NONESCAPE_OPERATOR;
                        //  TempLexeme.Value = "@";
                        //  TempData.Add(TempLexeme);
                        //
                        ///////////////////////////////////////////////////////////////////
                        ScPositionLast = ScPositionNow + 1;
                        break;
                    // "
                    case '\"':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        ScPositionLast = ScPositionNow;
                        if (ScPositionNow - 1 >= 0)
                            ScInString(ScOriginalCode[ScPositionNow - 1] != '@', true); // Process string and determine if escape
                        else
                            ScInString(true, true);
                        TempLexeme.Type = LexemeType.STRING_VALUE;
                        TempLexeme.Value = ScOriginalCode.Substring(ScPositionLast + 1, ScPositionNow - ScPositionLast - 1);
                        TempData.Add(TempLexeme);
                        ScPositionLast = ScPositionNow + 1;
                    break;
                    // (
                    case '(':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        TempLexeme.Type = LexemeType.LEFT_BRACKET;
                        TempLexeme.Value = "(";
                        TempLexeme.OperatorPriority = 11;
                        TempData.Add(TempLexeme);
                        ScPositionLast = ScPositionNow + 1;
                        break;
                    // )
                    case ')':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        TempLexeme.Type = LexemeType.RIGHT_BRACKET;
                        TempLexeme.Value = ")";
                        TempData.Add(TempLexeme);
                        ScPositionLast = ScPositionNow + 1;
                        break;
                    // {
                    case '{':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        ScPositionLast = ScPositionNow;
                        ScPositionNow += 1;
                        ScInBrace();
                        TempLexeme.Type = LexemeType.CODE_BLOCK;
                        TempLexeme.Value = ScOriginalCode.Substring(ScPositionLast + 1, ScPositionNow - ScPositionLast - 1);
                        TempData.Add(TempLexeme);
                        Lexeme End = new Lexeme();     
                        //////////////////////////////////////////////////////////////
                        //
                        //  Add an end-statement mark in order to make parsing easier.
                        //
                        //////////////////////////////////////////////////////////////
                        End.Value = ';';
                        End.Type = LexemeType.STATEMENT_END_MARK;
                        TempData.Add(End);
                        ScPositionLast = ScPositionNow + 1;
                        break;
                    // [
                    case '[':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        TempLexeme.Type = LexemeType.LEFT_SQUARE_BRACKET;
                        TempLexeme.Value = "[";
                        TempLexeme.OperatorPriority = 11;
                        TempData.Add(TempLexeme);
                        ScPositionLast = ScPositionNow + 1;
                        break;
                    // ]
                    case ']':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        TempLexeme.Type = LexemeType.RIGHT_SQUARE_BRACKET;
                        TempLexeme.Value = "]";
                        TempData.Add(TempLexeme);
                        ScPositionLast = ScPositionNow + 1;
                        break;
                    // ;
                    case ';':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        ScPositionLast = ScPositionNow + 1;
                        TempLexeme.Type = LexemeType.STATEMENT_END_MARK;
                        TempLexeme.Value = ";";
                        TempData.Add(TempLexeme);
                        break;
                    case ':':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        if (TempData.Count > 0 && TempData[TempData.Count - 1].Type == LexemeType.ID)
                        { 
                            ScPositionLast = ScPositionNow + 1;
                            TempLexeme.Type = LexemeType.COLON;
                            TempLexeme.Value = ":";
                            TempData.Add(TempLexeme);
                        }
                        else
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0006;
                            Error.ErrorDetail = "Symbol ':' must come after a ID.";
                            Error.LineNo = ScLineCount;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            ScStopScanning = true;
                        }
                        break;
                    // ~
                    case '~':
                        if (ScPositionNow - ScPositionLast >= 1)
                            TempData.Add(ScGetLast());
                        ScPositionLast = ScPositionNow + 1;
                        TempLexeme.Type = LexemeType.DESTRUCTOR_MARK;
                        TempLexeme.Value = "~";
                        TempData.Add(TempLexeme);
                        break;
                    // _ do nothing
                    case '_':
                        // regard as normal character.
                        break;
                    // When bug occoured
                    default:
                        if (char.IsPunctuation(ScOriginalCode[ScPositionNow]))
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0006;
                            Error.ErrorDetail = "Symbol '" + ScOriginalCode[ScPositionNow] + "' is not recogonized.";
                            Error.LineNo = ScLineCount;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error); 
                            ScStopScanning = true;
                        }
                        break;
                }
            }
            //////////////////////////////////////////////////////////////////////////////////////////////
            //
            //  Important! ScPositionNow should remains last index + 1.
            //  The last round while ScPositionNow >= ScOriginalCode.Length, the ScPositionNow 
            //  points to the last index + 1.
            //
            //////////////////////////////////////////////////////////////////////////////////////////////
            
            // Add the last lexeme
            if (ScPositionNow - ScPositionLast >= 0 && ScPositionLast < ScOriginalCode.Length)
                TempData.Add(ScGetLast());
            if (ScStopScanning)
                TempData.Clear();
            return TempData;
        } // ScScanW : List<Lexeme>

        /// <summary>
        /// Set the initial line. For use with recursive parsing.
        /// </summary>
        /// <param name="Line">Line number that will be set as the initial line.</param>
        public void ScSetInitialLineCount(int Line)
        {
            ScLineCount = Line;
        } // ScSetInitialLineCount : void
    }
}