/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1411281601
 Date of Generating: 2014/11/28
 File Name: ParseDeclaration.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Logic Parser.
  
 ==========================================================
 History:
 - 2014/11/28 16:01 : File created. Zhao Shixuan
 - 2015/02/18 11:29 : ParseIf finished. Zhao Shixuan
 - 2015/02/20 16:04 : ParseWhile finished. Zhao Shixuan
 - 2015/02/20 20:49 : ParseDoWhile finished. Zhao Shixuan
 - 2016/10/24 09:13 : Remastered and finished. Zhao SHixuan
 ===========================================================
 TODO:
 - Switch statements
***********************************************************/

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Parser
    {
        /// <summary>
        /// Parse if statement
        /// </summary>
        /// <returns>Parsed intermediate code</returns>
        private IntermediateCode PaParseIf()
        {
            //////////////////////////////////////////////////////////////////////////////
            //
            //    <if> <(...)> <{}> <StatementEndMark> [<else> <{}> <StatementEndMark>]
            //
            //////////////////////////////////////////////////////////////////////////////

            Error Error = new Error();
            IntermediateCode IC = new IntermediateCode();
            ICIfStatement IS = new ICIfStatement();
            IS.LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;
                        
            PaPositionEnd += 1;
            
            PaPositionNow = PaPositionEnd;
            // PaPositionNow -> (

            #region Condition

            // Assert the next lexeme must be a bracket
            if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.LEFT_BRACKET)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \"(\" is expected.";
                Error.LineNo = IS.LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // Return with error

            PaPositionNow += 1;
            PaPositionEnd = PaPositionNow;
            for (int BracketCount = 1; BracketCount > 0 && PaPositionEnd < PaLexemeStream.Count; PaPositionEnd++)
            {
                if (PaLexemeStream[PaPositionEnd].Type == LexemeType.LEFT_BRACKET)
                    BracketCount += 1;
                else if (PaLexemeStream[PaPositionEnd].Type == LexemeType.RIGHT_BRACKET)
                    BracketCount -= 1;
            }

            PaPositionEnd -= 1;
            if (PaPositionEnd >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.RIGHT_BRACKET)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \")\" is expected.";
                Error.LineNo = IS.LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // PaPositionNow -> the lexeme after (, PaPositionEnd -> )
            // Parse condition
            IntermediateCode ICCondition = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
            if (PaStopParsing)
                return IC;
            if (ICCondition.Type != ICStatementType.EXPRESSION)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A bool expression is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            IS.Condition = (ICExpression)ICCondition.StatementContent;
            PaPositionNow = PaPositionEnd;

            #endregion Condition

            // PaPositionNow -> )

            PaPositionNow += 1;
            PaPositionEnd = PaPositionNow;
            switch(PaLexemeStream[PaPositionNow].Type)
            {
                case LexemeType.ID:
                    int IDStart = PaPositionNow;
                    PaPositionNow += 1;
                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV1_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV2_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV3_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV4_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV5_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV6_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV7_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV8_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV10_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                    {
                        PaPositionEnd = PaPositionNow;
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        IS.TrueStatement = PaParseCommonExpression(IDStart, PaPositionEnd);
                        break;
                    }
                    else
                    {
                        Error.ID = ErrorID.C0011;
                        Error.ErrorDetail = "Cannot declear here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                case LexemeType.CHAR_VALUE:
                case LexemeType.FLOAT_VALUE:
                case LexemeType.INT_VALUE:
                case LexemeType.LV2_OPERATOR:
                case LexemeType.NEW:
                case LexemeType.STRING_VALUE:
                case LexemeType.TRUE:
                case LexemeType.FALSE:
                case LexemeType.LEFT_BRACKET:
                    for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                    IS.TrueStatement = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                    break;
                case LexemeType.CODE_BLOCK:
                    IS.TrueStatement = PaParseCodeBlock(PaPositionNow);
                    PaPositionNow += 1;
                    PaPositionEnd = PaPositionNow;
                    break;
                case LexemeType.IF:
                    IS.TrueStatement = PaParseIf();
                    break;
                case LexemeType.FOR:
                    IS.TrueStatement = PaParseFor();
                    break;
                case LexemeType.WHILE:
                    IS.TrueStatement = PaParseWhile();
                    break;
                case LexemeType.DO:
                    IS.TrueStatement = PaParseDoWhile();
                    break;
                case LexemeType.TRY:
                    IS.TrueStatement = PaParseTry();
                    break;
                case LexemeType.STATEMENT_END_MARK:
                    IntermediateCode Empty = new IntermediateCode();
                    Empty.Type = ICStatementType.EMPTY_STATEMENT;
                    IS.TrueStatement = Empty;
                    break;
                default:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "Cannot declear here.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
            }
            if (PaStopParsing)
                return IC;

            PaPositionEnd += 1;

            // If there's an "else"
            if (PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type == LexemeType.ELSE)
            {
                PaPositionEnd += 1;
                PaPositionNow = PaPositionEnd;
                ///////////////////////////////////
                // TODO OVERFLOW CHECK
                ///////////////////////////////////

                switch (PaLexemeStream[PaPositionNow].Type)
                {
                    case LexemeType.ID:
                        int IDStart = PaPositionNow;
                        PaPositionNow += 1;
                        if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV1_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV2_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV3_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV4_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV5_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV6_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV7_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV8_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LV10_OPERATOR
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                        {
                            PaPositionEnd = PaPositionNow;
                            for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                            IS.ElseStatement = PaParseCommonExpression(IDStart, PaPositionEnd);
                            break;
                        }
                        else
                        {
                            Error.ID = ErrorID.C0011;
                            Error.ErrorDetail = "Cannot declear here.";
                            Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }
                    case LexemeType.CHAR_VALUE:
                    case LexemeType.FLOAT_VALUE:
                    case LexemeType.INT_VALUE:
                    case LexemeType.LV2_OPERATOR:
                    case LexemeType.NEW:
                    case LexemeType.STRING_VALUE:
                    case LexemeType.TRUE:
                    case LexemeType.FALSE:
                    case LexemeType.LEFT_BRACKET:
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        IS.ElseStatement = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                        break;
                    case LexemeType.CODE_BLOCK:
                        IS.ElseStatement = PaParseCodeBlock(PaPositionNow);
                        PaPositionNow += 1;
                        PaPositionEnd = PaPositionNow;
                        break;
                    case LexemeType.IF:
                        IS.ElseStatement = PaParseIf();
                        break;
                    case LexemeType.FOR:
                        IS.ElseStatement = PaParseFor();
                        break;
                    case LexemeType.WHILE:
                        IS.ElseStatement = PaParseWhile();
                        break;
                    case LexemeType.DO:
                        IS.ElseStatement = PaParseDoWhile();
                        break;
                    case LexemeType.GOTO:
                        IS.ElseStatement = PaParseGoto();
                        break;
                    case LexemeType.TRY:
                        IS.ElseStatement = PaParseTry();
                        break;
                    case LexemeType.STATEMENT_END_MARK:
                        IntermediateCode Empty = new IntermediateCode();
                        Empty.Type = ICStatementType.EMPTY_STATEMENT;
                        IS.ElseStatement = Empty;
                        break;
                    default:
                        Error.ID = ErrorID.C0011;
                        Error.ErrorDetail = "Cannot declear here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                }
            }
            else
                PaPositionEnd -= 1;

            IC.Type = ICStatementType.IF_STATEMENT;
            IC.StatementContent = IS;

            return IC;
        }

        /// <summary>
        /// Parse for statement
        /// </summary>
        /// <returns>Parsed intermediate code</returns>
        private IntermediateCode PaParseFor()
        {
            Error Error = new Error();
            IntermediateCode IC = new IntermediateCode();
            IC.Type = ICStatementType.FOR_STATEMENT;
            ICForStatement FS;

            PaPositionNow = PaPositionEnd;            
            // PaPositionNow -> for

            PaPositionNow += 1;
            // PaPositionNow -> bracket

            // Assert the next lexeme must be a bracket
            if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.LEFT_BRACKET)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \"(\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // Returned error

            FS = PaParseForBracket();
            if (PaStopParsing)
                return IC;
            
            PaPositionNow = PaPositionEnd;
            // PaPositionNow -> the lexeme after the bracket

            switch (PaLexemeStream[PaPositionNow].Type)
            {
                case LexemeType.ID:
                    int IDStart = PaPositionNow;
                    PaPositionNow += 1;
                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV1_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV2_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV3_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV4_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV5_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV6_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV7_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV8_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV10_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                    {
                        PaPositionEnd = PaPositionNow;
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        FS.TrueStatement = PaParseCommonExpression(IDStart, PaPositionEnd);
                        break;
                    }
                    else
                    {
                        Error.ID = ErrorID.C0011;
                        Error.ErrorDetail = "Cannot declear here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                case LexemeType.CHAR_VALUE:
                case LexemeType.FLOAT_VALUE:
                case LexemeType.INT_VALUE:
                case LexemeType.LV2_OPERATOR:
                case LexemeType.NEW:
                case LexemeType.STRING_VALUE:
                case LexemeType.TRUE:
                case LexemeType.FALSE:
                case LexemeType.LEFT_BRACKET:
                    for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                    FS.TrueStatement = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                    break;
                case LexemeType.RIGHT_BRACKET:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "Unexpected \")\".";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                case LexemeType.CODE_BLOCK:
                    FS.TrueStatement = PaParseCodeBlock(PaPositionNow);
                    PaPositionNow += 1;
                    PaPositionEnd = PaPositionNow;
                    break;
                case LexemeType.IF:
                    FS.TrueStatement = PaParseIf();
                    break;
                case LexemeType.FOR:
                    FS.TrueStatement = PaParseFor();
                    break;
                case LexemeType.WHILE:
                    FS.TrueStatement = PaParseWhile();
                    break;
                case LexemeType.DO:
                    FS.TrueStatement = PaParseDoWhile();
                    break;
                case LexemeType.GOTO:
                    FS.TrueStatement = PaParseGoto();
                    break;
                case LexemeType.TRY:
                    FS.TrueStatement = PaParseTry();
                    break;
                case LexemeType.STATEMENT_END_MARK:
                    IntermediateCode Empty = new IntermediateCode();
                    Empty.Type = ICStatementType.EMPTY_STATEMENT;
                    FS.TrueStatement = Empty;
                    break;
                default:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "Cannot declear here.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
            }
            IC.StatementContent = FS;            
            return IC;
        }

        /// <summary>
        /// Initialize a intermediate code for for statement.
        /// </summary>
        /// <returns>A intermediate code of for statement that has already parsed the bracket of for.</returns>
        private ICForStatement PaParseForBracket()
        {
            ////////////////////////////////////////
            //
            //    [<...>] <;> [<...>] <;> [<...>]
            //
            ////////////////////////////////////////

            Error Error = new Error();

            ICForStatement FS = new ICForStatement();
            FS.LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;


            // PaPositionNow -> (
            PaPositionEnd = PaPositionNow + 1;
            // PaPositionEnd -> the next lexeme after (

            for(int BracketCount = 1; BracketCount > 0 && PaPositionEnd < PaLexemeStream.Count; PaPositionEnd++)
            {
                if (PaLexemeStream[PaPositionEnd].Type == LexemeType.LEFT_BRACKET)
                    BracketCount += 1;
                else if (PaLexemeStream[PaPositionEnd].Type == LexemeType.RIGHT_BRACKET)
                    BracketCount -= 1;
            }
            // PaPositionEnd -> the next lexeme after )

            if(PaPositionEnd == PaLexemeStream.Count)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \")\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return FS;
            }
            // Returned
            int EndOfBracket = PaPositionEnd;

            PaPositionEnd -= 1;
            // PaPositionEnd -> right bracket

            #region Initializer

            ICVariableDeclaration ICV = new ICVariableDeclaration();
            PaPositionNow += 1;
            switch (PaLexemeStream[PaPositionNow].Type)
            {
                case LexemeType.PUBLIC:
                case LexemeType.PROTECTED:
                case LexemeType.PRIVATE:
                case LexemeType.VIRTUAL:
                case LexemeType.OVERRIDE:
                case LexemeType.STATIC:
                case LexemeType.CONST:
                case LexemeType.FINAL:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "No modifiers allowed.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return FS;
                    // Returned
     
                case LexemeType.BOOL:
                    ICV.Type = 0;
                    goto Declarance;
                case LexemeType.INT:
                    ICV.Type = 1;
                    goto Declarance;
                case LexemeType.FLOAT:
                    ICV.Type = 2;
                    goto Declarance;
                case LexemeType.DOUBLE:
                    ICV.Type = 3;
                    goto Declarance;
                case LexemeType.CHAR:
                    ICV.Type = 4;
                    goto Declarance;
                case LexemeType.STRING:
                    ICV.Type = 5;
                    goto Declarance;
                    
                Declarance:
                    PaPositionNow += 1;

                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                    {
                        ICV.IsArray = true;
                        PaPositionNow += 1;

                        if (PaLexemeStream[PaPositionNow].Type == LexemeType.RIGHT_SQUARE_BRACKET)
                            ICV.IfArrayLengthDefined = false;
                        else if (PaLexemeStream[PaPositionNow].Type == LexemeType.INT_VALUE)
                        {
                            ICV.IfArrayLengthDefined = true;
                            ICV.ArrayLength = (int)PaLexemeStream[PaPositionNow].Value;

                            PaPositionNow += 1;


                            // Pair two square bracket. If not, return error.
                            if (PaLexemeStream[PaPositionNow].Type != LexemeType.RIGHT_SQUARE_BRACKET)
                            {
                                Error.ID = ErrorID.C0011;
                                Error.ErrorDetail = "Array's length must be only one constant integer.";
                                Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                PaStopParsing = true;
                                return FS;
                            }
                        }
                        else
                        {
                            Error.ID = ErrorID.C0011;
                            Error.ErrorDetail = "Array's length must be only one constant integer.";
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return FS;
                        }
                        PaPositionNow += 1;
                    }

                    if(PaLexemeStream[PaPositionNow].Type == LexemeType.ID)
                    {
                        ICV.Name = PaLexemeStream[PaPositionNow].Value.ToString();
                    }
                    else
                    {
                        Error.ID = ErrorID.C0011;
                        Error.ErrorDetail = "A name is expected.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return FS;
                    }

                    PaPositionNow += 1;

                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.STATEMENT_END_MARK)
                    {
                        ICV.IfDefined = false;
                        FS.Initializer.Type = ICStatementType.VARIABLE_DECLARATION;
                        FS.Initializer.StatementContent = (object)ICV;
                    }
                    else if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR && (Operators)PaLexemeStream[PaPositionNow].Value == Operators.ASSIGN)
                    {
                        PaPositionNow += 1;
                        ICV.IfDefined = true;
                        PaPositionEnd = PaPositionNow;
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        ICV.Definition = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                        FS.Initializer.Type = ICStatementType.VARIABLE_DECLARATION;
                        FS.Initializer.StatementContent = (object)ICV;
                    }
                    else
                    {
                        Error.ID = ErrorID.C0011;
                        Error.ErrorDetail = "Unexpected character \"" + PaLexemeStream[PaPositionNow].Value.ToString() + "\".";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return FS;
                    }

                    break;

                case LexemeType.ID:
                    PaPositionEnd = PaPositionNow;
                    int IDStart = PaPositionNow;
                    PaPositionNow += 1;
                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV1_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV2_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV3_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV4_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV5_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV6_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV7_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV8_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV10_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET)
                    {
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        FS.Initializer = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                        break;
                    }
                    else
                    {
                        ICV.Type = 6;
                        ICV.TypeName = (string)PaLexemeStream[PaPositionNow].Value;
                        goto Declarance;
                    }
                case LexemeType.CHAR_VALUE:
                case LexemeType.FLOAT_VALUE:
                case LexemeType.INT_VALUE:
                case LexemeType.LV2_OPERATOR:
                case LexemeType.NEW:
                case LexemeType.STRING_VALUE:
                case LexemeType.TRUE:
                case LexemeType.FALSE:
                case LexemeType.LEFT_BRACKET:
                    PaPositionEnd = PaPositionNow;
                    for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                    FS.Initializer = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                    break;
                case LexemeType.RIGHT_BRACKET:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "Unexpected \")\".";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return FS;
                case LexemeType.CODE_BLOCK:
                    PaPositionEnd = PaPositionNow;
                    FS.Initializer = PaParseCodeBlock(PaPositionNow);
                    PaPositionNow += 1;
                    PaPositionEnd = PaPositionNow;
                    break;
                case LexemeType.STATEMENT_END_MARK:
                    FS.Initializer = new IntermediateCode();
                    FS.Initializer.Type = ICStatementType.EMPTY_STATEMENT;
                    break;
                default:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "Unexpected.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return FS;
            }
            if (PaStopParsing)
                return FS;

            #endregion Initializer

            // PaPositionEnd -> StatementEndMark
            PaPositionEnd += 1;
            PaPositionNow = PaPositionEnd;


            #region Condition

            for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
            FS.Condition = (ICExpression)PaParseCommonExpression(PaPositionNow, PaPositionEnd).StatementContent;
            if (PaStopParsing)
                return FS;

            #endregion Condition

            // PositionEnd -> StatementEndMark
            PaPositionEnd += 1;
            PaPositionNow = PaPositionEnd;

            #region Iterator

            if (PaLexemeStream[PaPositionNow].Type != LexemeType.RIGHT_BRACKET)
                FS.Interator = (ICExpression)PaParseCommonExpression(PaPositionNow, EndOfBracket - 1).StatementContent;
            else
                FS.Interator = null;
            PaPositionEnd = EndOfBracket;

            #endregion Iterator

            // PaPositionEnd -> )

            return FS;
        }
        
        /// <summary>
        /// Parse while statement
        /// </summary>
        /// <returns>Parsed intermediate code</returns>
        private IntermediateCode PaParseWhile()
        {
            //////////////////////////////////////////////////
            //
            //   <while> <(...)> <{}> <StatementEndMark> 
            //
            //////////////////////////////////////////////////

            Error Error = new Error();
            IntermediateCode IC = new IntermediateCode();
            ICWhileStatement WS = new ICWhileStatement();
            
            PaPositionNow = PaPositionEnd;
        
            // PaPositionNow -> while
            PaPositionNow += 1;
            // PaPositionNow -> (

            #region Condition

            // Assert the next lexeme must be a bracket
            if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.LEFT_BRACKET)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \"(\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // Return with error

            PaPositionNow += 1;
            PaPositionEnd = PaPositionNow;
            for (int BracketCount = 1; BracketCount > 0 && PaPositionEnd < PaLexemeStream.Count; PaPositionEnd++)
            {
                if (PaLexemeStream[PaPositionEnd].Type == LexemeType.LEFT_BRACKET)
                    BracketCount += 1;
                else if (PaLexemeStream[PaPositionEnd].Type == LexemeType.RIGHT_BRACKET)
                    BracketCount -= 1;
            }
            PaPositionEnd -= 1;

            if (PaPositionEnd >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.RIGHT_BRACKET)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \")\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // PaPositionNow -> the lexeme after (, PaPositionEnd -> )
            // Parse condition
            IntermediateCode ICCondition = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
            if (PaStopParsing)
                return IC;
            if (ICCondition.Type != ICStatementType.EXPRESSION)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A bool expression is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            WS.Condition = (ICExpression)ICCondition.StatementContent;

            #endregion Condition

            // PaPositionEnd -> )
            PaPositionEnd += 1;
            PaPositionNow = PaPositionEnd;
            switch (PaLexemeStream[PaPositionNow].Type)
            {
                case LexemeType.ID:
                    int IDStart = PaPositionNow;
                    PaPositionNow += 1;
                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV1_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV2_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV3_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV4_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV5_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV6_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV7_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV8_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV10_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                    {
                        PaPositionEnd = PaPositionNow;
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        WS.TrueStatement = PaParseCommonExpression(IDStart, PaPositionEnd);
                        break;
                    }
                    else
                    {
                        Error.ID = ErrorID.C0011;
                        Error.ErrorDetail = "Cannot declear here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                case LexemeType.CHAR_VALUE:
                case LexemeType.FLOAT_VALUE:
                case LexemeType.INT_VALUE:
                case LexemeType.LV2_OPERATOR:
                case LexemeType.NEW:
                case LexemeType.STRING_VALUE:
                case LexemeType.TRUE:
                case LexemeType.FALSE:
                case LexemeType.LEFT_BRACKET:
                    for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                    WS.TrueStatement = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                    break;
                case LexemeType.CODE_BLOCK:
                    WS.TrueStatement = PaParseCodeBlock(PaPositionNow);
                    PaPositionNow += 1;
                    PaPositionEnd = PaPositionNow;
                    break;
                case LexemeType.IF:
                    WS.TrueStatement = PaParseIf();
                    break;
                case LexemeType.FOR:
                    WS.TrueStatement = PaParseFor();
                    break;
                case LexemeType.WHILE:
                    WS.TrueStatement = PaParseWhile();
                    break;
                case LexemeType.DO:
                    WS.TrueStatement = PaParseDoWhile();
                    break;
                case LexemeType.TRY:
                    WS.TrueStatement = PaParseTry();
                    break;
                case LexemeType.STATEMENT_END_MARK:
                    WS.TrueStatement = new IntermediateCode();
                    WS.TrueStatement.Type = ICStatementType.EMPTY_STATEMENT;
                    break;
                default:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "Cannot declear here.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
            }

            IC.Type = ICStatementType.WHILE_STATEMENT;
            IC.StatementContent = WS;

            return IC;
        }

        /// <summary>
        /// Parse do-while statement
        /// </summary>
        /// <returns>Parsed intermediate code</returns>
        private IntermediateCode PaParseDoWhile()
        {
            ///////////////////////////////////////////////////////////////////////
            //
            //   <do> <{}> <StatementEndMark> <while> <(...)> <StatementEndMark> 
            //
            ///////////////////////////////////////////////////////////////////////

            Error Error = new Error();
            IntermediateCode IC = new IntermediateCode();
            ICWhileStatement WS = new ICWhileStatement();

            // PaPositionEnd -> do
            PaPositionEnd += 1;
            PaPositionNow = PaPositionEnd;
            switch (PaLexemeStream[PaPositionNow].Type)
            {
                case LexemeType.ID:
                    int IDStart = PaPositionNow;
                    PaPositionNow += 1;
                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV1_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV2_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV3_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV4_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV5_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV6_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV7_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV8_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LV10_OPERATOR
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                    {
                        PaPositionEnd = PaPositionNow;
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        WS.TrueStatement = PaParseCommonExpression(IDStart, PaPositionEnd);
                        break;
                    }
                    else
                    {
                        Error.ID = ErrorID.C0011;
                        Error.ErrorDetail = "Cannot declear here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                case LexemeType.CHAR_VALUE:
                case LexemeType.FLOAT_VALUE:
                case LexemeType.INT_VALUE:
                case LexemeType.LV2_OPERATOR:
                case LexemeType.NEW:
                case LexemeType.STRING_VALUE:
                case LexemeType.TRUE:
                case LexemeType.FALSE:
                case LexemeType.LEFT_BRACKET:
                    for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                    WS.TrueStatement = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                    break;
                case LexemeType.CODE_BLOCK:
                    WS.TrueStatement = PaParseCodeBlock(PaPositionNow);
                    PaPositionNow += 1;
                    PaPositionEnd = PaPositionNow;
                    break;
                case LexemeType.IF:
                    WS.TrueStatement = PaParseIf();
                    break;
                case LexemeType.FOR:
                    WS.TrueStatement = PaParseFor();
                    break;
                case LexemeType.WHILE:
                    WS.TrueStatement = PaParseWhile();
                    break;
                case LexemeType.DO:
                    WS.TrueStatement = PaParseDoWhile();
                    break;
                case LexemeType.TRY:
                    WS.TrueStatement = PaParseTry();
                    break;
                case LexemeType.STATEMENT_END_MARK:
                    WS.TrueStatement = new IntermediateCode();
                    WS.TrueStatement.Type = ICStatementType.EMPTY_STATEMENT;
                    break;
                default:
                    Error.ID = ErrorID.C0011;
                    Error.ErrorDetail = "Cannot declear here.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
            }
            if (PaStopParsing)
                return IC;
            // PaPositionEnd -> STATE_END_MARK

            PaPositionEnd += 1;
            
            // Assert PaPositionEnd -> while
            if(PaLexemeStream[PaPositionEnd].Type != LexemeType.WHILE)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "\"while\" expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }

            PaPositionEnd += 1;
            PaPositionNow = PaPositionEnd;

            #region Condition

            // Assert the next lexeme must be a bracket
            if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.LEFT_BRACKET)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \"(\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // Return with error

            PaPositionNow += 1;
            PaPositionEnd = PaPositionNow;
            for (int BracketCount = 1; BracketCount > 0 && PaPositionEnd < PaLexemeStream.Count; PaPositionEnd++)
            {
                if (PaLexemeStream[PaPositionEnd].Type == LexemeType.LEFT_BRACKET)
                    BracketCount += 1;
                else if (PaLexemeStream[PaPositionEnd].Type == LexemeType.RIGHT_BRACKET)
                    BracketCount -= 1;
            }
            PaPositionEnd -= 1;

            if (PaPositionEnd >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.RIGHT_BRACKET)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \")\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // PaPositionNow -> the lexeme after (, PaPositionEnd -> )
            // Parse condition
            IntermediateCode ICCondition = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
            if (PaStopParsing)
                return IC;
            if (ICCondition.Type != ICStatementType.EXPRESSION)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A bool expression is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            WS.Condition = (ICExpression)ICCondition.StatementContent;
            PaPositionNow = PaPositionEnd;

            #endregion Condition

            // PaPositionNow == PaPositonENd -> )
            PaPositionEnd += 1;

            // Assert PaPositionEnd -> STATEMENT_END_MARK
            if (PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "\";\" expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }

            IC.Type = ICStatementType.DO_WHILE_STATEMENT;
            IC.StatementContent = WS;        

            return IC;
        }

        /// <summary>
        /// UNFINISHED. NO TOUCHY. UNUSED NOW. Parse switch sttatement
        /// </summary>
        /// <returns>Parsed Intermediate code</returns>
        private IntermediateCode PaParseSwitch()
        {
            IntermediateCode IC = new IntermediateCode();





            IC.Type = ICStatementType.SWITCH_STATEMENT;
            return IC;
        }

        /// <summary>
        /// Parse goto statement
        /// </summary>
        /// <returns>Parsed intermediate code</returns>
        private IntermediateCode PaParseGoto()
        {
            // PaPositionEnd -> goto
            Error Error = new Error();
            IntermediateCode IC = new IntermediateCode();
            ICGoto GS = new ICGoto();
            GS.LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;

            PaPositionEnd += 1;

            // Assert PaPositionEnd -> ID;
            if (PaPositionEnd >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.ID)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A label is expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // Return with error

            GS.Target = (string)PaLexemeStream[PaPositionEnd].Value;

            PaPositionEnd += 1;

            //Assert PaPositionEnd -> STATEMENT_END_MARK
            if (PaPositionEnd >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \";\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // Return with error
            IC.Type = ICStatementType.GOTO;
            IC.StatementContent = GS;
            return IC;
        }
        
        /// <summary>
        /// Parse goto target
        /// </summary>
        /// <returns>Parsed intermediate code</returns>
        private IntermediateCode PaParseGotoTarget()
        {
            IntermediateCode IC = new IntermediateCode();
            ICGotoTarget GTS = new ICGotoTarget();
            GTS.LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;

            GTS.Name = (string)PaLexemeStream[PaPositionEnd].Value;

            PaPositionEnd += 1;
            // Assert PaPositionEnd -> :
            if (PaPositionEnd >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.COLON)
            {
                Error Error = new Error();
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "A \":\" is expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            // Return with error

            IC.Type = ICStatementType.GOTO_TARGET;
            IC.StatementContent = GTS;
            return IC;
        }
    }
}
