using System.Collections.Generic;

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Parser
    {
        private IntermediateCode PaParseTry()
        {
            IntermediateCode IC = new IntermediateCode();
            IC.Type = ICStatementType.TRY_STATEMENT;
            ICTryStatement TS = new ICTryStatement();
            TS.LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;
            Error Error = new Error();

            // PaPositionEnd -> try
            PaPositionNow = ++PaPositionEnd;
            if (PaPositionEnd == PaLexemeStream.Count)
            {
                Error.ID = ErrorID.C0011;
                Error.ErrorDetail = "\";\" expected.";
                Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
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
                        || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET)
                    {
                        PaPositionEnd = PaPositionNow;
                        for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                        TS.Statements = PaParseCommonExpression(IDStart, PaPositionEnd);
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
                    TS.Statements = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                    break;
                case LexemeType.CODE_BLOCK:
                    TS.Statements = PaParseCodeBlock(PaPositionNow);
                    PaPositionNow += 1;
                    PaPositionEnd = PaPositionNow;
                    break;
                case LexemeType.IF:
                    TS.Statements = PaParseIf();
                    break;
                case LexemeType.FOR:
                    TS.Statements = PaParseFor();
                    break;
                case LexemeType.WHILE:
                    TS.Statements = PaParseWhile();
                    break;
                case LexemeType.DO:
                    TS.Statements = PaParseDoWhile();
                    break;
                case LexemeType.TRY:
                    TS.Statements = PaParseTry();
                    break;
                case LexemeType.STATEMENT_END_MARK:
                    TS.Statements = new IntermediateCode(); ;
                    break;
                default:
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "Cannot declear here.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
            }

            // PaPositionEnd -> STATEMENT_END_MARK
            PaPositionEnd += 1;

            // Assert the next lexeme must be "catch"
            if(PaPositionEnd == PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.CATCH)
            {
                Error.ID = ErrorID.C0013;
                Error.ErrorDetail = "\"catch\" expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }

            List<ICCatchStatement> CSList = new List<ICCatchStatement>();

            while (PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type == LexemeType.CATCH)
            {
                ICCatchStatement CS = new ICCatchStatement();
                CS.LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;
                PaPositionEnd += 1;
                // Assert the next lexeme must be "("
                if (PaPositionEnd == PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.LEFT_BRACKET)
                {
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "\"(\" expected.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }

                PaPositionEnd += 1;
                PaPositionNow = PaPositionEnd;
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
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "A \")\" is expected.";
                    Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }
                // PaPositionNow -> the lexeme after (, PaPositionEnd -> )
                ICVariableDeclaration Exception = new ICVariableDeclaration();
                // Assert PaLexemeStream[PaPositionEnd].Type == LexemeType.ID
                if(PaPositionNow == PaPositionEnd || PaLexemeStream[PaPositionNow].Type != LexemeType.ID)
                {
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "An expection name is expected.";
                    Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }
                Exception.Type = 6;
                Exception.TypeName = (string)PaLexemeStream[PaPositionNow].Value;
                PaPositionNow += 1;

                // Assert PaLexemeStream[PaPositionENd].Type == LexemeType.ID
                if (PaPositionNow == PaPositionEnd || PaLexemeStream[PaPositionNow].Type != LexemeType.ID)
                {
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "The name of the expection is expected.";
                    Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }
                Exception.Name = (string)PaLexemeStream[PaPositionNow].Value;
                CS.Exception = Exception;
                if (PaPositionNow + 1 != PaPositionEnd)
                {
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "\";\" expected.";
                    Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }

                PaPositionEnd += 1;

                // Assert PaLexemeStream still have lexemes
                if (PaPositionEnd == PaLexemeStream.Count)
                {
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "Statements are expected.";
                    Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }

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
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET)
                        {
                            PaPositionEnd = PaPositionNow;
                            for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                            CS.Actions = PaParseCommonExpression(IDStart, PaPositionEnd);
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
                        CS.Actions = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                        break;
                    case LexemeType.CODE_BLOCK:
                        CS.Actions = PaParseCodeBlock(PaPositionNow);
                        PaPositionNow += 1;
                        PaPositionEnd = PaPositionNow;
                        break;
                    case LexemeType.IF:
                        CS.Actions = PaParseIf();
                        break;
                    case LexemeType.FOR:
                        CS.Actions = PaParseFor();
                        break;
                    case LexemeType.WHILE:
                        CS.Actions = PaParseWhile();
                        break;
                    case LexemeType.DO:
                        CS.Actions = PaParseDoWhile();
                        break;
                    case LexemeType.TRY:
                        CS.Actions = PaParseTry();
                        break;
                    case LexemeType.STATEMENT_END_MARK:
                        CS.Actions = new IntermediateCode(); ;
                        break;
                    default:
                        Error.ID = ErrorID.C0013;
                        Error.ErrorDetail = "Cannot declear here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                }
                CSList.Add(CS);
                PaPositionEnd += 1;
            }

            TS.Catches = CSList;


            // Parse final
            // Check if there is a "final"
            if (PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type == LexemeType.FINALLY)
            {
                PaPositionEnd += 1;
                // Assert PaLexemeStream still have lexemes
                if (PaPositionEnd == PaLexemeStream.Count)
                {
                    Error.ID = ErrorID.C0013;
                    Error.ErrorDetail = "Statements are expected.";
                    Error.LineNo = PaLexemeStream[PaPositionEnd - 1].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }

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
                            || PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET)
                        {
                            PaPositionEnd = PaPositionNow;
                            for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                            TS.FinalActions = PaParseCommonExpression(IDStart, PaPositionEnd);
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
                        TS.FinalActions = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                        break;
                    case LexemeType.CODE_BLOCK:
                        TS.FinalActions = PaParseCodeBlock(PaPositionNow);
                        PaPositionNow += 1;
                        PaPositionEnd = PaPositionNow;
                        break;
                    case LexemeType.IF:
                        TS.FinalActions = PaParseIf();
                        break;
                    case LexemeType.FOR:
                        TS.FinalActions = PaParseFor();
                        break;
                    case LexemeType.WHILE:
                        TS.FinalActions = PaParseWhile();
                        break;
                    case LexemeType.DO:
                        TS.FinalActions = PaParseDoWhile();
                        break;
                    case LexemeType.TRY:
                        TS.FinalActions = PaParseTry();
                        break;
                    case LexemeType.STATEMENT_END_MARK:
                        TS.FinalActions = new IntermediateCode(); ;
                        break;
                    default:
                        Error.ID = ErrorID.C0013;
                        Error.ErrorDetail = "Cannot declear here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                }
            }

            IC.StatementContent = TS;
            return IC;
        }
    }
}
