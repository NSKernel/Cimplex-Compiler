/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1407311551
 Date of Generating: 2014/07/31
 File Name: _Lexeme.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Lexeme definition.
  
 ==========================================================
 History:
 - 2014/07/31 15:51 : File created. Zhao Shixuan
 - 2014/12/23 17:25 : Edited. Zhao Shixuan
 - 2016/10/24 09:13 : Edited.
                      Removed LINE_END_MARK by adding an
                      int value in each Lexeme.
                      Zhao Shixuan
***********************************************************/

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public struct Lexeme
    {
        public LexemeType Type;
        public object Value;
        public int OperatorPriority;

        public int LineNumber;
    }

    public enum LexemeType
    {
        ID,
        STATEMENT_END_MARK,
        DESTRUCTOR_MARK,

        CODE_BLOCK,
        LEFT_BRACKET,
        RIGHT_BRACKET,
        LEFT_SQUARE_BRACKET,
        RIGHT_SQUARE_BRACKET,

        COLON,

        INT_VALUE,
        FLOAT_VALUE,  // Included double value. The default is float.
        CHAR_VALUE,
        STRING_VALUE,

        // Operator
        LV1_OPERATOR, // NUMBER++, NUMBER--, PARENT.CHILD
        LV2_OPERATOR, // ++NUMBER, --NUMBER, !BOOL, +NUMBER, -NUMBER, new Constructor
        LV3_OPERATOR, // NUMBER * NUMBER, NUMBER / NUMBER, NUMBER % NUMBER
        LV4_OPERATOR, // NUMBER + NUMBER, NUMBER - NUMBER, STRING + STRING
        LV5_OPERATOR, // NUMBER < NUMBER, NUMBER > NUMBER, NUMBER <= NUMBER,
                      // NUMBER >= NUMBER
        LV6_OPERATOR, // NUMBER/CHAR/STRING/BOOL/ENUM.STUFF1 == NUMBER/CHAR/STRING/BOOL/ENUM.STUFF1
                      // !=
        LV7_OPERATOR, // BOOL && BOOL
        LV8_OPERATOR, // BOOL || BOOL
        LV9_OPERATOR, // Everything = Everything                              //////// 2333333333333
                      // NUMBER/STRING += NUMBER/STRING, NUMBER -= NUMBER
                      // NUMBER *= NUMBER, NUMBER /= NUMBER, NUMBER %= NUMBER
        LV10_OPERATOR,// STUFF1, STUFF2

        // Label
        LABEL_NAME,

        // Keywords
        //// Type
        NEW, 
        BOOL,

        ////// NUMBER
        INT,
        FLOAT,
        DOUBLE,

        CHAR,
        STRING,
        VOID,
        CLASS,
        // OPERATOR Not allowed!
        ENUM,

        //// Logic
        IF, 
        ELSE,
        DO,
        WHILE,
        FOR,
        BREAK,
        CONTINUE,
        GOTO,

        THIS,

        //// Modifier
        PUBLIC,
        PRIVATE,
        PROTECTED,
        VIRTUAL,
        OVERRIDE,
        STATIC,
        CONST,
        FINAL, // value applied only in the constructor or the declaration

        //// Values
        TRUE,
        FALSE,
        NULL,

        TRY,
        CATCH,
        FINALLY,
        THROW,
        
        RETURN,
    }
}
