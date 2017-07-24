using System;
using System.Collections.Generic;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public class IntermediateCode
    {
        public IntermediateCode()
        {
            Type = ICStatementType.EMPTY_STATEMENT;
        }

        public object StatementContent;
        public ICStatementType Type;
    }

    public enum ICStatementType
    {
        EMPTY_STATEMENT,
        
        CODE_BLOCK,

        VARIABLE_DECLARATION,
        METHOD_DECLARATION,
        CLASS_DECLARATION,
        CONSTRUCTOR_DECLARATION, // Still use the IC of method, but discard Name:string.
        DESTRUCTOR_DECLARATION,
        ENUM_DECLARATION,

        EXPRESSION,
        RETURN,

        IF_STATEMENT,
        FOR_STATEMENT,
        WHILE_STATEMENT,
        DO_WHILE_STATEMENT,
        SWITCH_STATEMENT,
        GOTO,
        GOTO_TARGET,
        
        TRY_STATEMENT,
    }

    #region IntermediateCodeStructures

    public class ICCodeBlock
    {
        public List<IntermediateCode> Contents = new List<IntermediateCode>();
        public SymbolTable Symbols = new SymbolTable();
    }

    #region Declaration

    /// <summary>
    /// Intermediate code structure for variable and array declarations
    /// </summary>
    public class ICVariableDeclaration
    {
        public int Accessbility = 0; // 0: private; 1: protected; 2: public. Private in default.
        // Inheriting while overriding requires modifier 'override'.
        public int Inherit = 0;   // 0: N/A; 1: UNEXPECTED; 2: override.        Only capable with methods
        public int EditControl = 0;  // 0: N/A; 1: static; 2: const; 3: final.  Normal in default
        public int Type;             // 0: bool; 1: int; 2: float; 3: double; 4: char; 5: string; 6: id(an instanced class).
                                     // Not initialized to prevent forgetting setting the value.
        public int LineNumber;

        public string TypeName
        {
            get 
            {
                if (Type == 6)
                    return IDName;
                else
                    throw new ArgumentException("Cannot get the name except ID.");
            }
            set
            {
                if (Type == 6)
                    IDName = value;
                else
                    throw new ArgumentException("Cannot specific the name except ID.");
            }
        }
        private string IDName = "";

        public string Name;

        public bool IsArray = false;

        #region Define

        public bool IfDefined = false;
        public IntermediateCode Definition;
        public bool IfArrayLengthDefined = false;
        public int ArrayLength = 0;

        #endregion
    }

    /// <summary>
    /// Intermediate code structure for method declarations
    /// </summary>
    public class ICMethodDeclaration
    {
        public int Accessbility = 0; // 0: private; 1: protected; 2: protected. Private in default.
        public int Inherit = 0;      // 0: N/A; 1: virtual; 2: override.        
        public int EditControl = 0;  // 0: N/A; 1: static; 2: const; 3: final.  Normal in default
        public int Type;             // 0: bool; 1: int; 2: float; 3: double; 4: char; 5: string; 6: id(an instanced class).
        // Not initialized to prevent forgetting setting the value.

        public int LineNumber;

        public string TypeName
        {
            get
            {
                if (Type == 6)
                    return IDName;
                else
                    throw new ArgumentException("Cannot specific the name except ID.");
            }
            set
            {
                if (Type == 6)
                    IDName = value;
                else
                    throw new ArgumentException("Cannot specific the name except ID.");
            }
        }
        private string IDName = "";

        public string Name;

        public List<ICVariableDeclaration> ArgumentList = new List<ICVariableDeclaration>();

        public bool IsArray = false;

        #region Define

        public bool IfDefined = false;
        public List<IntermediateCode> Definition = new List<IntermediateCode>();
        
        // Array's length is not allowed to be defined.
        // public bool IfArrayLengthDefined = false;
        // public int ArrayLength = 0;

        #endregion
    }

    /// <summary>
    /// Intermediate code structure for class declarations
    /// </summary>
    public class ICClassDeclaration
    {
        
        public int Accessbility = 0; // 0: private; 1: protected; 2: protected. Private in default.
        public string Name;
        public List<Lexeme> Ancestors = new List<Lexeme>();
        public List<ICMethodDeclaration> Constructors = new List<ICMethodDeclaration>();
        public ICMethodDeclaration Destructor = null;

        public int LineNumber;

        public List<IntermediateCode> Declarations = new List<IntermediateCode>(); // Can only insist of declarations!
    }

    /// <summary>
    /// Intermediate code structure for enum declarations
    /// </summary>
    public class ICEnumDeclaration
    {
        public int Accessbility = 0; // 0: private; 1: protected; 2: protected. Private in default.
        // Not capable! Not a method!
        // public int Inherit = 0;   // 0: N/A; 1: virtual; 2: override.        Only capable with methods
        // Not capable!
        // public int EditControl = 0;  // 0: N/A; 1: static;                      Normal in default
        public string Name;

        public int LineNumber;

        // Must be defined
        // private bool IfDefined = false;
        public List<string> Items = new List<string>(); // Can only insist of Labels!
    }

    #endregion Declaration

    #region Expression

    /// <summary>
    /// Intermediate code structure for BASIC EXPRESSION!
    /// </summary>
    public class ICExpression
    {
        public string LValueName;
        public ICExpression Operand1;
        public Operators Operator;
        public ICExpression Operand2;

        public object Value;

        public int ReturnType;
        public string ReturnTypeName;

        public int LineNumber;
    }

    

    #endregion Expression

    #region Logic

    public class ICIfStatement
    {
        public ICIfStatement()
        {
            // Set default
            TrueStatement.Type = ICStatementType.EMPTY_STATEMENT;
            ElseStatement.Type = ICStatementType.EMPTY_STATEMENT;
        }

        public ICExpression Condition = new ICExpression();
        
        public int LineNumber;

        public IntermediateCode TrueStatement = new IntermediateCode();
        public IntermediateCode ElseStatement = new IntermediateCode();
    }

    public class ICForStatement
    {
        public ICForStatement()
        {
            TrueStatement.Type = ICStatementType.EMPTY_STATEMENT;
        }

        public IntermediateCode Initializer = new IntermediateCode();
        public ICExpression Condition = new ICExpression();
        public ICExpression Interator = new ICExpression();

        public int LineNumber;

        public IntermediateCode TrueStatement = new IntermediateCode();
    }

    /// <summary>
    /// While and Do-While statements
    /// </summary>
    public class ICWhileStatement
    {
        // Solved with statement type
        // public bool IfDo = false;

        public ICExpression Condition = new ICExpression();
        
        public int LineNumber;

        public IntermediateCode TrueStatement = new IntermediateCode();
    }

    public class ICGoto
    {
        public string Target;
        public int LineNumber;
    }

    public class ICGotoTarget
    {
        public string Name;
        public int LineNumber;
    }

    #endregion Logic

    #region ExceptionProcessing

    public class ICTryStatement
    {
        public IntermediateCode Statements = new IntermediateCode();

        public int LineNumber;

        public List<ICCatchStatement> Catches = new List<ICCatchStatement>();
        public IntermediateCode FinalActions = new IntermediateCode();
    }

    public class ICCatchStatement
    {
        public int LineNumber;

        public ICVariableDeclaration Exception = new ICVariableDeclaration();
        public IntermediateCode Actions = new IntermediateCode();
    }

    #endregion ExceptionProcessing

    #endregion IntermediateCodeStructures

    public class Domain
    {
        public SymbolTable Symbols = new SymbolTable();
        public List<IntermediateCode> Code;
    }
}

