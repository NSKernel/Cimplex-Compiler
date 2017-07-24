/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1411281558
 Date of Generating: 2014/11/28
 File Name: ParseDeclaration.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Declaration Parser.
  
 ==========================================================
 History:
 - 2014/11/28 15:58 : File created. Zhao Shixuan
 --------- Million times of debugging and editing ---------
 - 2015/02/03 12:16 : 0.0.0.1 Debug version finished.
                              808 lines, お疲れ様～
                              Yeah~
                              Zhao Shixuan
 - 2015/06/05 22:23 : 0.0.0.2 Debug version rearranged.
                              Adapted for new scanner which
                              stops treating brackets and
                              square brackets as a united
                              object. 
                              832 lines in total.
                              Zhao Shixuan
***********************************************************/

using System.Collections.Generic;

using NanoStudio.Internel.Cimplex.Compiler.ErrorHandler;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public partial class Parser
    {
        private IntermediateCode PaParseDeclaration()
        {
            int Accessibility = 0;                  // 0: private; 1: protected; 2: public. Private in default.
            // bool AccessbilityEdited = false;
            int AccessbilityAssignmentLine = 0;

            int Inherit = 0;                        // 0: N/A; 1: virtual; 2: override. Normal in default.
            // bool InheritEdited = false;
            int InheritAssignmentLine = 0;

            int EditControl = 0;                    // 0: N/A; 1: static; 2: const; 3: final.  Normal in default
            // bool EditControlEdited = false;
            int EditControlAssignmentLine = 0;

            int Type;                               // 0: bool; 1: int; 2: float; 3: double; 4: char; 5: string; 6: id(an instanced class); 7:enmu; 8: class;
            string TypeName = "";
            int TypeAssignmentLine = 0;             // For future use(abstract classes). Now it makes no sense.

            bool IsArray = false;                   // To show if the declared variable or method is an array.
            bool IfArrayLengthDefined = false;
            int ArrayLength = 0;

            string Name = "";
            int Mode = 0;                           // 0: Variable; 1: Method; 2. Class; 3. Enum; 4. Array.

            int LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;

            bool IsConstructor = false;

            IntermediateCode IC = new IntermediateCode();
            try
            {
                // During the 'for' in PaParse, the PaPositionEnd is automatically added one so that it points to the first lexeme of the next statement.
                // Set PaPositionNow to the first lexeme of the current statement.
                PaPositionNow = PaPositionEnd;

                #region ParseModifier

                // Not necessary. The method is called based on the first lexeme so that it can't be Line_Marker.
                // PaJumpLineMarksNow();

                // Access control
                if (PaLexemeStream[PaPositionNow].Type == LexemeType.PRIVATE)
                {
                    PaPositionNow += 1;
                    // AccessbilityEdited = true;
                    AccessbilityAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                else if (PaLexemeStream[PaPositionNow].Type == LexemeType.PROTECTED)
                {
                    Accessibility = 1;
                    PaPositionNow += 1;
                    // AccessbilityEdited = true;
                    AccessbilityAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                else if (PaLexemeStream[PaPositionNow].Type == LexemeType.PUBLIC)
                {
                    Accessibility = 2;
                    PaPositionNow += 1;
                    // AccessbilityEdited = true;
                    AccessbilityAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                // else: fall through

                // InheriteControl
                // N/A needs nothing to do.
                if (PaLexemeStream[PaPositionNow].Type == LexemeType.VIRTUAL)
                {
                    Inherit = 1;
                    PaPositionNow += 1;
                    // InheritEdited = true;
                    InheritAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                else if (PaLexemeStream[PaPositionNow].Type == LexemeType.OVERRIDE)
                {
                    Inherit = 2;
                    PaPositionNow += 1;
                    // InheritEdited = true;
                    EditControlAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                // else: fall through

                // EditControl
                // N/A needs nothing to do.
                if (PaLexemeStream[PaPositionNow].Type == LexemeType.STATIC)
                {
                    EditControl = 1;
                    PaPositionNow += 1;
                    // EditControlEdited = true;
                    EditControlAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                else if (PaLexemeStream[PaPositionNow].Type == LexemeType.CONST)
                {
                    EditControl = 2;
                    PaPositionNow += 1;
                    // EditControlEdited = true;
                    EditControlAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                else if (PaLexemeStream[PaPositionNow].Type == LexemeType.FINAL)
                {
                    EditControl = 3;
                    PaPositionNow += 1;
                    // EditControlEdited = true;
                    EditControlAssignmentLine = PaLexemeStream[PaPositionNow].LineNumber;
                }
                // else: fall through

                #endregion ParseModifier
                // PaPositionNow points to the lexeme after all modifiers which should be type.

                #region ParseType

                // Type
                // If not, throw bug.
                switch (PaLexemeStream[PaPositionNow].Type)
                {
                    case LexemeType.BOOL:
                        Type = 0;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.INT:
                        Type = 1;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.FLOAT:
                        Type = 2;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.DOUBLE:
                        Type = 3;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.CHAR:
                        Type = 4;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.STRING:
                        Type = 5;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.ID:
                        Type = 6;
                        TypeName = (string)PaLexemeStream[PaPositionNow].Value;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.ENUM:
                        Type = 7;
                        Mode = 3;
                        PaPositionNow += 1;
                        break;
                    case LexemeType.CLASS:
                        Type = 7;
                        Mode = 2; // CAN BE DETERMINED NOW. 
                        PaPositionNow += 1;
                        break;


                    // Modifier position error:
                    case LexemeType.PUBLIC:
                    case LexemeType.PRIVATE:
                    case LexemeType.PROTECTED:
                    case LexemeType.VIRTUAL:
                    case LexemeType.OVERRIDE:
                    case LexemeType.STATIC:
                    case LexemeType.CONST:
                    case LexemeType.FINAL:
                        Error ErrorMod = new Error();
                        ErrorMod.ID = ErrorID.C0010;
                        ErrorMod.ErrorDetail = "Invalid \"" + PaLexemeStream[PaPositionNow].Value.ToString() + "\". Modifier must be declared in order.";
                        ErrorMod.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(ErrorMod);
                        PaStopParsing = true;
                        return IC;
                    default:
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "A declaration must have a type.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                }
                // else: Must throw error

                TypeAssignmentLine = PaLexemeStream[PaPositionNow - 1].LineNumber;

                #endregion ParseType
                // PaPositionNow points to the lexeme after type which should be ID.

                if (PaPositionNow < PaLexemeStream.Count && PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET)
                {
                    IsConstructor = true;
                    goto ParseMethod;
                }

                // ArrayParsing.
                #region ArrayParsing

                if (PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                {
                    IsArray = true;

                    #region ArrayLengthParsing

                    PaPositionNow += 1;

                    if (PaLexemeStream[PaPositionNow].Type == LexemeType.RIGHT_SQUARE_BRACKET)
                        IfArrayLengthDefined = false;
                    else if (PaLexemeStream[PaPositionNow].Type == LexemeType.INT_VALUE)
                    {
                        IfArrayLengthDefined = true;
                        ArrayLength = (int)PaLexemeStream[PaPositionNow].Value;

                        PaPositionNow += 1;

                        // Pair two square bracket. If not, return error.
                        if (PaLexemeStream[PaPositionNow].Type != LexemeType.RIGHT_SQUARE_BRACKET)
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0010;
                            Error.ErrorDetail = "Array's length must be only one constant integer.";
                            Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }
                    }
                    else
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "Array's length must be only one constant integer.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }

                    #endregion ArrayLengthParsing

                    PaPositionNow += 1;
                }

                #endregion ArrayParsing
                // Continue or error

                // Parse Name
                #region ParseName

                if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.ID)
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0010;
                    Error.ErrorDetail = "A declaration must have a name comes after the type.";
                    Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }
                else
                {
                    Name = (string)PaLexemeStream[PaPositionNow].Value;
                    PaPositionNow++;
                }

                #endregion ParseID
                // PaPositionNow points to the lexeme after ID.


                #region ClassParsing

                if (Mode == 2)
                {
                    ICClassDeclaration CD = new ICClassDeclaration();
                    CD.Accessbility = Accessibility;
                    CD.Name = Name;
                    CD.LineNumber = LineNumber;
                    // Process inheritive
                    if (PaPositionNow < PaLexemeStream.Count && PaLexemeStream[PaPositionNow].Type == LexemeType.COLON)
                    {
                        PaPositionNow += 1;
                        while (PaPositionNow < PaLexemeStream.Count && PaLexemeStream[PaPositionNow].Type == LexemeType.ID)
                        {
                            CD.Ancestors.Add(PaLexemeStream[PaPositionNow]);
                            PaPositionNow += 1;
                            // Assert the next lexeme is a comma. If not, break;
                            if (PaPositionNow < PaLexemeStream.Count && PaLexemeStream[PaPositionNow].Type != LexemeType.LV10_OPERATOR)
                            {
                                break;
                            }
                            else
                                PaPositionNow += 1;
                        }
                    }

                    if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.CODE_BLOCK /*  || PaLexemeStream[PaPositionNow + 1].Type != LexemeType.STATEMENT_END_MARK Assert that 
                                                                                                                                 it must have been ended. The statement end mark is automatically added to the stream by the Scanner */
                                                                                                                             )
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "The declaration of class is illegal. '{' is expected.";
                        Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                    else
                    {

                        if (EditControl != 0)
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0010;
                            Error.ErrorDetail = "The declaration of class cannot setup an edit control level.";
                            Error.LineNo = EditControlAssignmentLine;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }
                        if (Inherit != 0)
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0010;
                            Error.ErrorDetail = "The declaration of class cannot setup an inheriting modifier.";
                            Error.LineNo = InheritAssignmentLine;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }
                        if (IsArray)
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0010;
                            Error.ErrorDetail = "The declaration of class is illegal.";
                            Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }

                        CD.Declarations = new List<IntermediateCode>();

                        Scanner ScanClassContents = new Scanner((string)PaLexemeStream[PaPositionNow].Value);
                        ScanClassContents.ScSetInitialLineCount(PaLexemeStream[PaPositionNow].LineNumber);

                        Parser ParseClassContents = new Parser(ScanClassContents.ScScan());
                        ParseClassContents.PaForceTreatIDAsDeclaration = true;
                        CD.Declarations = ParseClassContents.PaParse();

                        foreach (IntermediateCode CheckingIC in CD.Declarations)
                        {
                            if (CheckingIC.Type == ICStatementType.CONSTRUCTOR_DECLARATION)
                                CD.Constructors.Add((ICMethodDeclaration)CheckingIC.StatementContent);
                            else if (CheckingIC.Type == ICStatementType.DESTRUCTOR_DECLARATION)
                            {
                                if (CD.Destructor == null)
                                    CD.Destructor = (ICMethodDeclaration)CheckingIC.StatementContent;
                                else
                                {
                                    Error Error = new Error();
                                    Error.ID = ErrorID.C0010;
                                    Error.ErrorDetail = "A class can only have one destructor.";
                                    Error.LineNo = ((ICMethodDeclaration)CheckingIC.StatementContent).LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            else if (CheckingIC.Type != ICStatementType.VARIABLE_DECLARATION && CheckingIC.Type != ICStatementType.METHOD_DECLARATION && CheckingIC.Type != ICStatementType.ENUM_DECLARATION)
                            {
                                Error Error = new Error();
                                Error.ID = ErrorID.C0010;
                                Error.ErrorDetail = "A class can only consist of variables and methods.";
                                Error.LineNo = CD.LineNumber;
                                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                PaStopParsing = true;
                                return IC;
                            }

                        }
                        IC.StatementContent = CD;
                        IC.Type = ICStatementType.CLASS_DECLARATION;
                        PaPositionEnd = PaPositionNow + 1;
                        return IC;
                    }
                }

                #endregion ClassParsing
                // Returned or error.

                // EnumParsing.
                #region EnumParsing

                if (Mode == 3)
                {
                    if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.CODE_BLOCK)
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "The declaration of enum is not legal. '{' is expected.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                    else
                    {
                        ICEnumDeclaration ED = new ICEnumDeclaration();
                        ED.Accessbility = Accessibility;
                        ED.Name = Name;
                        ED.LineNumber = LineNumber;

                        if (EditControl != 0)
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0010;
                            Error.ErrorDetail = "The declaration of enum cannot setup an edit control level.";
                            Error.LineNo = EditControlAssignmentLine;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }
                        if (Inherit != 0)
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0010;
                            Error.ErrorDetail = "The declaration of enum cannot setup an inheriting modifier.";
                            Error.LineNo = InheritAssignmentLine;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }
                        if (IsArray)
                        {
                            Error Error = new Error();
                            Error.ID = ErrorID.C0010;
                            Error.ErrorDetail = "The declaration of enum is illegal.";
                            Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                            ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                            PaStopParsing = true;
                            return IC;
                        }


                        ED.Items = new List<string>();
                        Scanner ScanEnumContents = new Scanner((string)PaLexemeStream[PaPositionNow].Value);
                        List<Lexeme> LabelTokens = new List<Lexeme>();
                        ScanEnumContents.ScSetInitialLineCount(PaLexemeStream[PaPositionNow].LineNumber);
                        LabelTokens = ScanEnumContents.ScScan();
                        for (int i = 0; i < LabelTokens.Count; i++)
                        {
                            if (i < LabelTokens.Count && LabelTokens[i].Type == LexemeType.ID)
                            {
                                ED.Items.Add((string)LabelTokens[i].Value);
                                i += 1;

                                //   If i is larger than LabelTokens.Count, that means every label is scanned
                                // and no more ',' is required.
                                if (i < LabelTokens.Count && LabelTokens[i].Type != LexemeType.LV10_OPERATOR)
                                {
                                    Error Error = new Error();
                                    Error.ID = ErrorID.C0010;
                                    Error.ErrorDetail = "The declaration of enum is not legal. A ',' is expected.";
                                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                    PaStopParsing = true;
                                    return IC;
                                }
                            }
                            else
                            {
                                Error Error = new Error();
                                Error.ID = ErrorID.C0010;
                                Error.ErrorDetail = "The declaration of enum is not legal. A label is expected.";
                                Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                                PaStopParsing = true;
                                return IC;
                            }
                        }
                        PaPositionEnd = PaPositionNow + 1;
                        IC.StatementContent = ED;
                        IC.Type = ICStatementType.ENUM_DECLARATION;
                        return IC;
                    }
                }

            #endregion EnumParsing
            // Returned or error.

            ParseMethod:

                // MethodParsing. Only defiened methods are valid now. Need Improvement
                #region MethodParsing

                if (PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_BRACKET)
                {
                    ICMethodDeclaration MD = new ICMethodDeclaration();
                    MD.Accessbility = Accessibility;
                    MD.EditControl = EditControl;
                    MD.Inherit = Inherit;
                    MD.Type = Type;
                    MD.LineNumber = LineNumber;
                    if (Type == 6)
                        MD.TypeName = TypeName;
                    MD.Name = Name;
                    PaPositionNow += 1;
                    MD.ArgumentList = PaParseArgumentList();
                    MD.IsArray = IsArray;
                    if (IsArray && IfArrayLengthDefined)
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "The length cannot be declared here.";
                        Error.LineNo = EditControlAssignmentLine;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                    if (PaPositionNow >= PaLexemeStream.Count || PaLexemeStream[PaPositionNow].Type != LexemeType.CODE_BLOCK)
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "The declaration of method is illegal. Need and only need a code block.";
                        Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        return IC;
                    }
                    else
                    {
                        // PaPositionNow -> CodeBlock
                        MD.IfDefined = true;
                        MD.Definition = new List<IntermediateCode>();

                        Scanner ScanMethodContents = new Scanner((string)PaLexemeStream[PaPositionNow].Value);
                        ScanMethodContents.ScSetInitialLineCount(PaLexemeStream[PaPositionNow].LineNumber);

                        Parser ParseMethodContents = new Parser(ScanMethodContents.ScScan());
                        MD.Definition = ParseMethodContents.PaParse();
                        PaPositionEnd = PaPositionNow + 1; // PaPositionEnd = PaPositionNow + 1 -> STATEMENT_END_MARK
                        IC.StatementContent = MD;
                        if (IsConstructor)
                            IC.Type = ICStatementType.CONSTRUCTOR_DECLARATION;
                        else
                            IC.Type = ICStatementType.METHOD_DECLARATION;
                        return IC;
                    }
                }

                #endregion MethodParsing
                // Returned or error

                ICVariableDeclaration VD = new ICVariableDeclaration();

                IC.Type = ICStatementType.VARIABLE_DECLARATION;
                VD.Accessbility = Accessibility;
                VD.EditControl = EditControl;
                VD.Inherit = Inherit;
                VD.Type = Type;
                VD.LineNumber = LineNumber;
                if (VD.Type == 6)
                    VD.TypeName = TypeName;

                VD.Name = Name;
                VD.IsArray = IsArray;
                VD.IfArrayLengthDefined = IfArrayLengthDefined;
                VD.ArrayLength = ArrayLength;




                if (PaLexemeStream[PaPositionNow].Type == LexemeType.STATEMENT_END_MARK)
                {
                    IC.StatementContent = (object)VD;
                    return IC;
                }
                else if (PaLexemeStream[PaPositionNow].Type == LexemeType.LV9_OPERATOR && (Operators)PaLexemeStream[PaPositionNow].Value == Operators.ASSIGN)
                {
                    PaPositionNow += 1;
                    VD.IfDefined = true;
                    PaPositionEnd = PaPositionNow;
                    for (; PaPositionEnd < PaLexemeStream.Count && PaLexemeStream[PaPositionEnd].Type != LexemeType.STATEMENT_END_MARK; PaPositionEnd++) ;
                    VD.Definition = PaParseCommonExpression(PaPositionNow, PaPositionEnd);
                    IC.StatementContent = (object)VD;
                    return IC;
                }
                else
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0010;
                    Error.ErrorDetail = "Unexpected character \"" + PaLexemeStream[PaPositionNow].Value.ToString() + "\".";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    return IC;
                }

                /////////////////////////////////////////////
                //
                //   Assert: The code will never get here.
                //
                /////////////////////////////////////////////
            }
            catch (System.Exception)
            {
                Error Error = new Error();
                Error.ID = ErrorID.C0010;
                Error.ErrorDetail = "The declaration parsing now meets an unhandled problem. Please contact the developer of Cimplex for bug reporting.";
                Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
        }

        private List<ICVariableDeclaration> PaParseArgumentList()
        {
            List<ICVariableDeclaration> ReturnList = new List<ICVariableDeclaration>();

            int EndOfArgumentList = PaPositionNow;
            for (int BracketCount = 1; BracketCount > 0 && EndOfArgumentList < PaLexemeStream.Count; EndOfArgumentList++)
            {
                if (PaLexemeStream[EndOfArgumentList].Type == LexemeType.LEFT_BRACKET)
                    BracketCount += 1;
                else if (PaLexemeStream[EndOfArgumentList].Type == LexemeType.RIGHT_BRACKET)
                    BracketCount -= 1;
            }
            EndOfArgumentList -= 1;


            if (PaLexemeStream[EndOfArgumentList].Type != LexemeType.RIGHT_BRACKET)
            {
                Error Error = new Error();
                Error.ID = ErrorID.C0010;
                Error.ErrorDetail = "A \')\' is expected.";
                Error.LineNo = PaLexemeStream[EndOfArgumentList].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return ReturnList;
            }

            for (; PaPositionNow < EndOfArgumentList;)
            {
                ICVariableDeclaration VD = new ICVariableDeclaration();

                // If no more arguments, return.
                if (PaPositionNow == EndOfArgumentList)
                    return ReturnList;

                #region ParseType

                switch (PaLexemeStream[PaPositionNow].Type)
                {
                    case LexemeType.ID:
                        VD.Type = 7;
                        VD.TypeName = (string)PaLexemeStream[PaPositionNow].Value;
                        break;
                    case LexemeType.BOOL:
                        VD.Type = 0;
                        break;
                    case LexemeType.INT:
                        VD.Type = 1;
                        break;
                    case LexemeType.FLOAT:
                        VD.Type = 2;
                        break;
                    case LexemeType.DOUBLE:
                        VD.Type = 3;
                        break;
                    case LexemeType.CHAR:
                        VD.Type = 4;
                        break;
                    case LexemeType.STRING:
                        VD.Type = 5;
                        break;
                    default:
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "Argument declaration is not valid. No modifier is allowed and a type is required.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        break;
                }

                VD.LineNumber = PaLexemeStream[PaPositionNow].LineNumber;
                PaPositionNow += 1;

                #region LineJumpAndOutOfRangeProcess

                if (PaPositionNow >= EndOfArgumentList)
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0010;
                    Error.ErrorDetail = "Argument declaration is not finished. A name is expected.";
                    Error.LineNo = PaLexemeStream[EndOfArgumentList].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    break;
                }

                #endregion LineJumpAndOutOfRangeProcess

                #endregion ParseType
                // PaPositionNow points to the lexeme after the type

                if (PaLexemeStream[PaPositionNow].Type == LexemeType.LEFT_SQUARE_BRACKET)
                {
                    VD.IsArray = true;
                    if (PaPositionNow >= EndOfArgumentList)
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "A \']\' is expected.";
                        Error.LineNo = PaLexemeStream[EndOfArgumentList].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        break;
                    }
                    else if (PaLexemeStream[PaPositionNow + 1].Type != LexemeType.RIGHT_SQUARE_BRACKET)
                    {
                        Error Error = new Error();
                        Error.ID = ErrorID.C0010;
                        Error.ErrorDetail = "The length of array cannot be declared here.";
                        Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                        ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                        PaStopParsing = true;
                        break;
                    }
                    PaPositionNow += 2;
                }

                #region LineJumpAndOutOfRangeProcess

                if (PaPositionNow >= EndOfArgumentList)
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0010;
                    Error.ErrorDetail = "Argument declaration is not finished. A name is expected.";
                    Error.LineNo = PaLexemeStream[EndOfArgumentList].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    break;
                }

                #endregion LineJumpAndOutOfRangeProcess

                if (PaLexemeStream[PaPositionNow].Type == LexemeType.ID)
                {
                    VD.Name = (string)PaLexemeStream[PaPositionNow].Value;
                    PaPositionNow += 1;
                }
                else
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0010;
                    Error.ErrorDetail = "Argument declaration is not valid. A name is expected.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    break;
                }

                ReturnList.Add(VD);

                // If IndexNow is larger than LabelTokens.Count, that means every label is scanned
                // and no more ',' is required.
                if (PaPositionNow < EndOfArgumentList && PaLexemeStream[PaPositionNow].Type != LexemeType.LV10_OPERATOR)
                {
                    Error Error = new Error();
                    Error.ID = ErrorID.C0010;
                    Error.ErrorDetail = "The declaration of arguments is not legal. A ',' is expected.";
                    Error.LineNo = PaLexemeStream[PaPositionNow].LineNumber;
                    ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                    PaStopParsing = true;
                    break;
                }
                PaPositionNow += 1;
            }

            return ReturnList;
        }

        private IntermediateCode PaParseDestructor()
        {
            Error Error = new Error();
            IntermediateCode IC = new IntermediateCode();
            ICMethodDeclaration MD = new ICMethodDeclaration();
            MD.LineNumber = PaLexemeStream[PaPositionEnd].LineNumber;
            // PaPositionEnd -> ~
            PaPositionEnd += 1;

            // Assert the next lexeme is an id
            if(PaPositionEnd == PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.ID)
            {
                Error.ID = ErrorID.C0010;
                Error.ErrorDetail = "The name of the class is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }

            MD.Type = 6;
            MD.TypeName = (string)PaLexemeStream[PaPositionEnd].Value;

            PaPositionEnd += 1;

            // Destructor does not have parameters
            if(PaPositionEnd + 1 >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.LEFT_BRACKET || PaLexemeStream[PaPositionEnd + 1].Type != LexemeType.RIGHT_BRACKET)
            {
                Error.ID = ErrorID.C0010;
                Error.ErrorDetail = "\"()\" expected and no parameter allowed for a destructor.";
                Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            PaPositionEnd += 2;

            // Assert the next lexeme is a code block
            if (PaPositionEnd >= PaLexemeStream.Count || PaLexemeStream[PaPositionEnd].Type != LexemeType.CODE_BLOCK)
            {
                Error.ID = ErrorID.C0010;
                Error.ErrorDetail = "The definition of the destructor is expected.";
                Error.LineNo = PaLexemeStream[PaPositionNow - 1].LineNumber;
                ErrorHandler.ErrorHandler.EhErrorOutput(Error);
                PaStopParsing = true;
                return IC;
            }
            Scanner ScanMethodContents = new Scanner((string)PaLexemeStream[PaPositionEnd].Value);
            ScanMethodContents.ScSetInitialLineCount(PaLexemeStream[PaPositionEnd].LineNumber);

            Parser ParseMethodContents = new Parser(ScanMethodContents.ScScan());
            MD.Definition = ParseMethodContents.PaParse();
            PaPositionEnd += 1; // PaPositionEnd += 1 -> STATEMENT_END_MARK
            IC.StatementContent = MD;
            IC.Type = ICStatementType.DESTRUCTOR_DECLARATION;
            return IC;
        }
    }
}
