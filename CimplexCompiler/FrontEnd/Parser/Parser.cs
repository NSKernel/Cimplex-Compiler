/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1409191452
 Date of Generating: 2014/09/19
 File Name: Parser.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Parser Main.
  
 ==========================================================
 History:
 - 2014/09/19 14:52 : File created. Zhao Shixuan
 - 2015/06/05 22:23 : 0.0.0.1 Debug version.
                              Zhao Shixuan
 - 2016/10/24 09:13 : 0.0.0.2 Debug version.
                              Zhao Shixuan
***********************************************************/

using System.Collections.Generic;

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Parser
    {
        /// <summary>
        /// Original lexeme stream
        /// </summary>
        private List<Lexeme> PaLexemeStream = new List<Lexeme>();
        
        /// <summary>
        /// Pointer to the end of the statement
        /// </summary>
        private int PaPositionEnd = 0;

        /// <summary>
        /// Pointer to the position now
        /// </summary>
        private int PaPositionNow = 0;

        /// <summary>
        /// Show if it should stop parsing, use with errors
        /// </summary>
        private bool PaStopParsing = false;

        /// <summary>
        /// A trick used in parsing class.
        /// </summary>
        private bool PaForceTreatIDAsDeclaration = false;

        /// <summary>
        /// The Cimplex Language Syntactic Parser Object Constructor
        /// </summary>
        /// <param name="LexemeStream">The lexeme stream to be parsed.</param>
        public Parser(List<Lexeme> LexemeStream)
        {
            PaLexemeStream = LexemeStream;
        } // Parser : Constructor

        /// <summary>
        /// Parse the lexeme stream.
        /// </summary>
        /// <returns>Parsed intermediate code list.</returns>
        public List<IntermediateCode> PaParse()
        {
            return PaParseW();
        } // PaParse : List<IntermediateCode>

        /// <summary>
        /// Parser worker method.
        /// </summary>
        /// <returns>Parsed intermediate code list.</returns>
        private List<IntermediateCode> PaParseW()
        {
            List<IntermediateCode> ReturnList = new List<IntermediateCode>();
            for(; PaPositionEnd < PaLexemeStream.Count && !PaStopParsing; PaPositionEnd++)
            {
                switch(PaLexemeStream[PaPositionEnd].Type)
                {
                    #region Declaration

                    // Parse Declaration
                    case LexemeType.PUBLIC:
                    case LexemeType.PROTECTED:
                    case LexemeType.PRIVATE:
                    case LexemeType.VIRTUAL:
                    case LexemeType.OVERRIDE:
                    case LexemeType.STATIC:
                    case LexemeType.CONST:
                    case LexemeType.FINAL:
                    case LexemeType.BOOL:
                    case LexemeType.INT:
                    case LexemeType.FLOAT:
                    case LexemeType.DOUBLE:
                    case LexemeType.CHAR:
                    case LexemeType.STRING:
                    case LexemeType.VOID:
                    case LexemeType.CLASS:
                    case LexemeType.ENUM:
                        ReturnList.Add(PaParseDeclaration());
                        break;

                    case LexemeType.DESTRUCTOR_MARK:
                        ReturnList.Add(PaParseDestructor());
                        break;

                    #endregion Declaration

                    #region StartsWithID

                    // 1. Expression
                    // 2. Declaration
                    case LexemeType.ID:
                        int IDPosition = PaPositionEnd + 1;
                        if(IDPosition == PaLexemeStream.Count)
                        {
                            Error ErrorG = new Error();
                            ErrorG.ID = ErrorID.C0009;
                            ErrorG.ErrorDetail = "\";\" expected.";
                            ErrorG.LineNo = PaLexemeStream[PaPositionEnd].LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(ErrorG);
                            PaStopParsing = true;
                            break;
                        }
                        if (PaLexemeStream[IDPosition].Type == LexemeType.LV1_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV2_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV3_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV4_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV5_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV6_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV7_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV8_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV9_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LV10_OPERATOR
                            || PaLexemeStream[IDPosition].Type == LexemeType.LEFT_SQUARE_BRACKET)
                            ReturnList.Add(PaParseExpression());
                        else if(PaLexemeStream[IDPosition].Type == LexemeType.LEFT_BRACKET)
                        {
                            if (PaForceTreatIDAsDeclaration)
                                ReturnList.Add(PaParseDeclaration());
                            else
                                ReturnList.Add(PaParseExpression());
                        }
                        else if (PaLexemeStream[IDPosition].Type == LexemeType.COLON)
                            ReturnList.Add(PaParseGotoTarget());
                        else if (PaLexemeStream[IDPosition].Type == LexemeType.STATEMENT_END_MARK)
                        {
                            Error ErrorG = new Error();
                            ErrorG.ID = ErrorID.C0009;
                            ErrorG.ErrorDetail = "A statement is required.";
                            ErrorG.LineNo = PaLexemeStream[PaPositionEnd].LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(ErrorG);
                            PaStopParsing = true;
                            break;
                        }
                        else
                            ReturnList.Add(PaParseDeclaration());
                        break;

                    #endregion StartWithID

                    case LexemeType.CHAR_VALUE:
                    case LexemeType.FLOAT_VALUE:
                    case LexemeType.INT_VALUE:
                    case LexemeType.LV2_OPERATOR:
                    case LexemeType.NEW:
                    case LexemeType.STRING_VALUE:
                    case LexemeType.TRUE:
                    case LexemeType.FALSE:
                    case LexemeType.LEFT_BRACKET:
                        ReturnList.Add(PaParseExpression());
                        break;
                    case LexemeType.RIGHT_BRACKET:
                        Error Error = new Error();
                        Error.ID = ErrorID.C0009;
                        Error.ErrorDetail = "Unexpected \")\".";
                        Error.LineNo = PaLexemeStream[PaPositionEnd].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        break;
                    case LexemeType.CODE_BLOCK:
                        ReturnList.Add(PaParseCodeBlock(PaPositionEnd));
                        PaPositionEnd += 1;
                        break;
                    case LexemeType.IF:
                        ReturnList.Add(PaParseIf());
                        break;
                    case LexemeType.FOR:
                        ReturnList.Add(PaParseFor());
                        break;
                    case LexemeType.WHILE:
                        ReturnList.Add(PaParseWhile());
                        break;
                    case LexemeType.DO:
                        ReturnList.Add(PaParseDoWhile());
                        break;
                    case LexemeType.GOTO:
                        ReturnList.Add(PaParseGoto());
                        break;
                    case LexemeType.TRY:
                        ReturnList.Add(PaParseTry());
                        break;
                    case LexemeType.LABEL_NAME:
                        break;
                    case LexemeType.RETURN:
                        ReturnList.Add(PaParseReturn());
                        break;
                    case LexemeType.STATEMENT_END_MARK:
                        IntermediateCode Empty = new IntermediateCode();
                        Empty.Type = ICStatementType.EMPTY_STATEMENT;
                        ReturnList.Add(Empty);
                        break;
                    default:
                        Error ErrorT = new Error();
                        ErrorT.ID = ErrorID.C0009;
                        ErrorT.ErrorDetail = "Unexpected \"" + PaLexemeStream[PaPositionEnd].Type.ToString() +"\".";
                        ErrorT.LineNo = PaLexemeStream[PaPositionEnd].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(ErrorT);
                        PaStopParsing = true;
                        break;
                }
            }
            return ReturnList;
        } // PaParseW : List<IntermediateCode>
    }
}
