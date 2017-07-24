/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1503271528
 Date of Generating: 2015/03/27
 File Name: _Common.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Common definitions of the compiler.
  
 ==========================================================
 History:
 - 2015/03/27 15:28 : File created. Zhao Shixuan
***********************************************************/

namespace NanoStudio.Internel.Cimplex.Compiler
{
    /// <summary>
    /// Operators that appear in the expression
    /// </summary>
    public enum Operators
    {
        // = =
        ID,               // Operand2 will be discard.
        CHAR_VALUE,
        FLOAT_VALUE,
        INT_VALUE,
        STRING_VALUE,
        BOOL_VALUE,

        // LV1
        GET_CHILD,          // PARENT.CHILD
        FUNCTION_CALL,      // NAME(ARGUMENTS)
                            // Operand1 will be the function name while
                            // Operand2 will be the argument list
        INCREASE_POSTFIX,   // NUMBER++
        DECREASE_POSTFIX,   // NUMBER--
        ARRAY,              // NAME[VALUE]

        // LV2
        OPPSITE,            // !BOOL
        POSITIVE,           // +NUMBER
        NEGTIVE,            // -NUMBER
        INCREASE_PREFIX,    // ++NUMBER
        DECREASE_PREFIX,    // --NUMBER
        TYPE_CAST,          // (TYPE)ID, (TYPE)(...), (TYPE)CONST, (ID)ID, (ID)(...), (ID)CONST
        NEW,                // new Constructor

        // LV3
        MULTIPLICATION,     // NUMBER * NUMBER
        DIVISION,           // NUMBER / NUMBER
        MOD,                // NUMBER % NUMBER

        // LV4
        ADDITION,           // NUMBER + NUMBER
                            // STRING + STRING
        SUBTRACTION,        // NUMBER - NUMBER

        // LV5 COMPARE
        LESS_THAN,          // NUMBER < NUMBER
        GREATER_THAN,       // NUMBER > NUMBER
        LESS_OR_EQUAL,      // NUMBER <= NUMBER
        GREATER_OR_EQUAL,   // NUMBER >= NUMBER

        // LV6
        EQUAL,              // STUFF == STUFF
        NOT_EQUAL,          // STUFF != STUFF

        // LV7
        AND,                // BOOL && BOOL

        // LV8
        OR,                 // BOOL || BOOL

        // LV9
        ASSIGN,
        ADD_ASSIGN,
        SUB_ASSIGN,
        MUL_ASSIGN,
        DIV_ASSIGN,
        MOD_ASSIGN,
        
        // LV10
        COMMA,
    }
}
