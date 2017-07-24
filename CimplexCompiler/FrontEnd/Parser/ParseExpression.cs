/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1411281603
 Date of Generating: 2014/11/28
 File Name: ParseExpression.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Parse expression.
  
 ==========================================================
 History:
 - 2014/11/28 16:03 : File created. Zhao Shixuan
 - 2015/03/13 15:53 : Started...editing? Zhao Shixuan
***********************************************************/

using System.Collections.Generic;

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Parser
    {
        public IntermediateCode PaParseExpression()
        {
            PaPositionNow = PaPositionEnd;
            for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;

            return PaParseCommonExpression(PaPositionNow, PaPositionEnd);
        }


        /// <summary>
        /// A reference based expression parser
        /// </summary>
        /// <param name="StartPoint">Parser from.</param>
        /// <param name="EndPoint">Parse ends at. Usually ";", ")"</param>
        /// <returns></returns>
        public IntermediateCode PaParseCommonExpression(int StartPoint, int EndPoint)
        {
            ///////////////////////////////////////////
            //
            //   Should parse until PaPositionEnd - 1
            //   as PaPositionEnd points to ';' or ')'
            //   or some other stuff like that.
            //
            ///////////////////////////////////////////
            //
            //   Shutting-yard Algorithm
            //
            ///////////////////////////////////////////

            Error Error = new Error();
            IntermediateCode IC = new IntermediateCode();
            IC.Type = ICStatementType.EXPRESSION;
            Stack<Lexeme> OperatorStack = new Stack<Lexeme>();
            Stack<ICExpression> OutputStack = new Stack<ICExpression>();
            Stack<bool> BracketIfFunction = new Stack<bool>();

            #region Shutting-Yard Body

            int LastPosition = -1;
            for (; StartPoint < EndPoint; StartPoint++)
            {
                ICExpression ExpNow = new ICExpression();
                ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                ExpNow.Operand1 = null;
                ExpNow.Operand2 = null;
                ExpNow.LValueName = null;                
                switch (PaLexemeStream[StartPoint].Type)
                {
                    // Values
                    case LexemeType.CHAR_VALUE:
                        ExpNow.Value = PaLexemeStream[StartPoint].Value;
                        ExpNow.Operator = Operators.CHAR_VALUE;
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;
                    case LexemeType.FLOAT_VALUE:
                        ExpNow.Value = PaLexemeStream[StartPoint].Value;
                        ExpNow.Operator = Operators.FLOAT_VALUE;
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;                   
                    case LexemeType.INT_VALUE:
                        ExpNow.Value = PaLexemeStream[StartPoint].Value;
                        ExpNow.Operator = Operators.INT_VALUE;
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;
                    case LexemeType.STRING_VALUE:
                        ExpNow.Value = PaLexemeStream[StartPoint].Value;
                        ExpNow.Operator = Operators.STRING_VALUE;
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;
                    case LexemeType.TRUE:
                        ExpNow.Value = true;
                        ExpNow.Operator = Operators.BOOL_VALUE;
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;
                    case LexemeType.FALSE:
                        ExpNow.Value = false;
                        ExpNow.Operator = Operators.BOOL_VALUE;
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;

                    // ID -> 1. Variable. 2. Function. 3. Type cast
                    case LexemeType.ID:
                        ExpNow.Value = PaLexemeStream[StartPoint].Value;
                        ExpNow.Operator = Operators.ID;
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;

                    // Array
                    case LexemeType.LEFT_SQUARE_BRACKET:
                        OperatorStack.Push(PaLexemeStream[StartPoint]);
                        LastPosition = StartPoint;
                        break;
                    case LexemeType.RIGHT_SQUARE_BRACKET:
                        if (OperatorStack.Peek().Type == LexemeType.RIGHT_SQUARE_BRACKET)
                        {
                            ExpNow.Operator = Operators.ARRAY;
                            OutputStack.Push(ExpNow);
                            OperatorStack.Pop();
                            LastPosition = StartPoint;
                            break;
                        }
                        while (OperatorStack.Count > 0 && OperatorStack.Peek().Type != LexemeType.LEFT_SQUARE_BRACKET)
                        {
                            if (OperatorStack.Peek().OperatorPriority > 2)
                            {
                                if (OutputStack.Count >= 2)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand2 = OutputStack.Pop();
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = ExpNow.LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            else
                            {
                                if((Operators)OperatorStack.Peek().Value == Operators.TYPE_CAST)
                                {
                                    if (OutputStack.Count >= 2)
                                    {
                                        ExpNow = new ICExpression();
                                        ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                        ExpNow.Operand2 = OutputStack.Pop();
                                        ExpNow.Operand1 = OutputStack.Pop();
                                        ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                    }
                                    else
                                    {
                                        Error.ID = ErrorID.C0012;
                                        Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                        Error.LineNo = ExpNow.LineNumber;
                                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                        PaStopParsing = true;
                                        return IC;
                                    }
                                }
                                else if (OutputStack.Count >= 1)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operand2 = null;
                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = ExpNow.LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            OutputStack.Push(ExpNow);
                        }
                        if (OperatorStack.Count == 0)
                        {
                            Error.ID = ErrorID.C0012;
                            Error.ErrorDetail = "Unexpected \"]\".";
                            Error.LineNo = ExpNow.LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }

                        OperatorStack.Pop();

                        ExpNow = new ICExpression();
                        ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                        ExpNow.Operator = Operators.ARRAY;
                        ExpNow.Operand2 = OutputStack.Pop();
                        ExpNow.Operand1 = OutputStack.Pop();
                        OutputStack.Push(ExpNow);
                        LastPosition = StartPoint;
                        break;

                    // Operators
                    case LexemeType.LV1_OPERATOR:
                    // case LexemeType.LV2_OPERATOR:
                    case LexemeType.LV3_OPERATOR:
                    case LexemeType.LV4_OPERATOR:
                    case LexemeType.LV5_OPERATOR:
                    case LexemeType.LV6_OPERATOR:
                    case LexemeType.LV7_OPERATOR:
                    case LexemeType.LV8_OPERATOR:
                    case LexemeType.LV10_OPERATOR:
                        while(OperatorStack.Count > 0 && OperatorStack.Peek().OperatorPriority <= PaLexemeStream[StartPoint].OperatorPriority)
                        {

                            if (OperatorStack.Peek().OperatorPriority > 2)
                            {
                                if (OutputStack.Count >= 2)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand2 = OutputStack.Pop();
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            else
                            {
                                if ((Operators)OperatorStack.Peek().Value == Operators.TYPE_CAST || (Operators)OperatorStack.Peek().Value == Operators.GET_CHILD)
                                {
                                    if (OutputStack.Count >= 2)
                                    {
                                        ExpNow = new ICExpression();
                                        ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                        ExpNow.Operand2 = OutputStack.Pop();
                                        ExpNow.Operand1 = OutputStack.Pop();
                                        ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                    }
                                    else
                                    {
                                        Error.ID = ErrorID.C0012;
                                        Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                        Error.LineNo = ExpNow.LineNumber;
                                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                        PaStopParsing = true;
                                        return IC;
                                    }
                                }
                                else if (OutputStack.Count >= 1)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operand2 = null;

                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            OutputStack.Push(ExpNow);
                        }
                        OperatorStack.Push(PaLexemeStream[StartPoint]);
                        LastPosition = StartPoint;
                        break;

                    case LexemeType.LV9_OPERATOR:
                    case LexemeType.LV2_OPERATOR:
                        while (OperatorStack.Count > 0 && OperatorStack.Peek().OperatorPriority < PaLexemeStream[StartPoint].OperatorPriority)
                        {
                            if (OperatorStack.Peek().OperatorPriority > 2)
                            {
                                if (OutputStack.Count >= 2)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand2 = OutputStack.Pop();
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            else
                            {
                                if ((Operators)OperatorStack.Peek().Value == Operators.TYPE_CAST || (Operators)OperatorStack.Peek().Value == Operators.GET_CHILD)
                                {
                                    if (OutputStack.Count >= 2)
                                    {
                                        ExpNow = new ICExpression();
                                        ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                        ExpNow.Operand2 = OutputStack.Pop();
                                        ExpNow.Operand1 = OutputStack.Pop();
                                        ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                    }
                                    else
                                    {
                                        Error.ID = ErrorID.C0012;
                                        Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                        Error.LineNo = ExpNow.LineNumber;
                                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                        PaStopParsing = true;
                                        return IC;
                                    }
                                }
                                else if (OutputStack.Count >= 1)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operand2 = null;
                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            OutputStack.Push(ExpNow);
                        }
                        OperatorStack.Push(PaLexemeStream[StartPoint]);
                        LastPosition = StartPoint;
                        break;

                    case LexemeType.LEFT_BRACKET:
                        if (LastPosition >= 0 && PaLexemeStream[LastPosition].Type == LexemeType.ID)
                            BracketIfFunction.Push(true);
                        else
                            BracketIfFunction.Push(false);
                        OperatorStack.Push(PaLexemeStream[StartPoint]);
                        LastPosition = StartPoint;
                        break;
                    case LexemeType.RIGHT_BRACKET:
                        if (LastPosition > 0 && PaLexemeStream[LastPosition].Type == LexemeType.LEFT_BRACKET)
                        {
                            if (BracketIfFunction.Pop())
                            {
                                ExpNow.Operator = Operators.FUNCTION_CALL;
                                ExpNow.Operand2 = null;
                                ExpNow.Operand1 = OutputStack.Pop();
                                OutputStack.Push(ExpNow);
                                OperatorStack.Pop();
                                LastPosition = StartPoint;
                                break;
                            }
                            else
                            {
                                Error.ID = ErrorID.C0012;
                                Error.ErrorDetail = "Unexpected \")\".";
                                Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                PaStopParsing = true;
                                return IC;
                            }
                        }
                        while(OperatorStack.Count > 0 && OperatorStack.Peek().Type != LexemeType.LEFT_BRACKET)
                        {
                            if (OperatorStack.Peek().OperatorPriority > 2)
                            {
                                if (OutputStack.Count >= 2)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand2 = OutputStack.Pop();
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            else
                            {
                                if ((Operators)OperatorStack.Peek().Value == Operators.TYPE_CAST || (Operators)OperatorStack.Peek().Value == Operators.GET_CHILD)
                                {
                                    if (OutputStack.Count >= 2)
                                    {
                                        ExpNow = new ICExpression();
                                        ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                        ExpNow.Operand2 = OutputStack.Pop();
                                        ExpNow.Operand1 = OutputStack.Pop();
                                        ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                    }
                                    else
                                    {
                                        Error.ID = ErrorID.C0012;
                                        Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                        Error.LineNo = ExpNow.LineNumber;
                                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                        PaStopParsing = true;
                                        return IC;
                                    }
                                }
                                else if (OutputStack.Count >= 1)
                                {
                                    ExpNow = new ICExpression();
                                    ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                                    ExpNow.Operand1 = OutputStack.Pop();
                                    ExpNow.Operand2 = null;
                                    ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                                }
                                else
                                {
                                    Error.ID = ErrorID.C0012;
                                    Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                                    Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            OutputStack.Push(ExpNow);
                        }
                        if(OperatorStack.Count == 0)
                        {
                            Error.ID = ErrorID.C0012;
                            Error.ErrorDetail = "Unexpected \")\".";
                            Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }

                        OperatorStack.Pop();

                        if (BracketIfFunction.Pop())
                        {
                            ExpNow = new ICExpression();
                            ExpNow.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                            ExpNow.Operator = Operators.FUNCTION_CALL;
                            ExpNow.Operand2 = OutputStack.Pop();
                            ExpNow.Operand1 = OutputStack.Pop();
                            OutputStack.Push(ExpNow);
                        }
                        else if (StartPoint + 1 < PaLexemeStream.Count
                            && (PaLexemeStream[StartPoint + 1].Type != LexemeType.LV1_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV2_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV3_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV4_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV5_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV6_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV7_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV8_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV9_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.LV10_OPERATOR &&
                                PaLexemeStream[StartPoint + 1].Type != LexemeType.STATEMENT_END_MARK
                            ))
                        {
                            Lexeme TypeCast = new Lexeme();
                            TypeCast.LineNumber = PaLexemeStream[StartPoint].LineNumber;
                            TypeCast.Type = LexemeType.LV2_OPERATOR;
                            TypeCast.OperatorPriority = 2;
                            TypeCast.Value = Operators.TYPE_CAST;
                            OperatorStack.Push(TypeCast);
                        }
                        LastPosition = StartPoint;
                        break;

                    case LexemeType.CODE_BLOCK:
                        // Can and can only return an array
                        Scanner CodeBlockScanner = new Scanner((string)PaLexemeStream[StartPoint].Value);
                        CodeBlockScanner.ScSetInitialLineCount(PaLexemeStream[StartPoint].LineNumber);
                        List<Lexeme> ScannedCodeBlock = CodeBlockScanner.ScScan();
                        Parser CodeBlockParser = new Parser(ScannedCodeBlock);
                        OutputStack.Push((ICExpression)CodeBlockParser.PaParseCommonExpression(0, ScannedCodeBlock.Count).StatementContent);
                        if (PaStopParsing)
                            return IC;
                        LastPosition = StartPoint;
                        break;

                    case LexemeType.STATEMENT_END_MARK:
                        Error.ID = ErrorID.C0012;
                        Error.ErrorDetail = "Unexpeted \";\".";
                        Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    default:
                        Error.ID = ErrorID.C0012;
                        Error.ErrorDetail = "Unexpeted \"" + PaLexemeStream[StartPoint].Type.ToString() + "\".";
                        Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                }
            }

            #endregion Shutting-Yard Body

            while (OperatorStack.Count > 0)
            {
                ICExpression ExpNow = new ICExpression();
                if (OperatorStack.Peek().OperatorPriority > 2)
                {
                    if (OutputStack.Count >= 2)
                    {
                        ExpNow = new ICExpression();
                        ExpNow.LineNumber = OperatorStack.Peek().LineNumber;
                        ExpNow.Operand2 = OutputStack.Pop();
                        ExpNow.Operand1 = OutputStack.Pop();
                        ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                    }
                    else
                    {
                        Error.ID = ErrorID.C0012;
                        Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                        Error.LineNo = OperatorStack.Peek().LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                }
                else if(OperatorStack.Peek().Type == LexemeType.LEFT_BRACKET)
                {
                    Error.ID = ErrorID.C0012;
                    Error.ErrorDetail = "Unexpected \"(\".";
                    Error.LineNo = OperatorStack.Peek().LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }
                else if (OperatorStack.Peek().Type == LexemeType.LEFT_SQUARE_BRACKET)
                {
                    Error.ID = ErrorID.C0012;
                    Error.ErrorDetail = "Unexpected \"[\".";
                    Error.LineNo = OperatorStack.Peek().LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }
                else
                {
                    if (OutputStack.Count >= 1)
                    {
                        ExpNow = new ICExpression();
                        ExpNow.LineNumber = OperatorStack.Peek().LineNumber;
                        ExpNow.Operand1 = OutputStack.Pop();
                        ExpNow.Operand2 = null;
                        ExpNow.Operator = (Operators)OperatorStack.Pop().Value;
                    }
                    else
                    {
                        Error.ID = ErrorID.C0012;
                        Error.ErrorDetail = "Unexpected \"" + PaLexemeStream[StartPoint].Value.ToString() + "\".";
                        Error.LineNo = PaLexemeStream[StartPoint].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                }
                OutputStack.Push(ExpNow);
            }

            if (OutputStack.Count > 1)
            {
                Error.ID = ErrorID.C0012;
                Error.ErrorDetail = "\";\" expected.";
                Error.LineNo = OutputStack.Peek().LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            else if(OutputStack.Count == 0)
            {
                return IC;
            }
            else
                IC.StatementContent = OutputStack.Pop();
            return IC;
        }

        private IntermediateCode PaParseCodeBlock(int Position)
        {
            Scanner CodeBlockScanner = new Scanner((string)PaLexemeStream[Position].Value);
            CodeBlockScanner.ScSetInitialLineCount(PaLexemeStream[Position].LineNumber);
            Parser CodeBlockParser = new Parser(CodeBlockScanner.ScScan());
            IntermediateCode IC = new IntermediateCode();
            IC.StatementContent = CodeBlockParser.PaParse();
            IC.Type = ICStatementType.CODE_BLOCK;
            return IC;
        }

        private IntermediateCode PaParseReturn()
        {
            IntermediateCode IC = new IntermediateCode();
            Error Error = new Error();
            PaPositionNow = PaPositionEnd;
            for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;

            PaPositionNow += 1;
            if (PaPositionNow < PaPositionEnd)
                IC.StatementContent = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
            else
            {
                Error.ID = ErrorID.C0012;
                Error.ErrorDetail = "Expect a return expression.";
                Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            if (PaStopParsing)
                return IC;
            IC.Type = ICStatementType.RETURN;
            return IC;
        }
    }
}
