/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1611030952
 Date of Generating: 2016/11/03
 File Name: SemanticAnalyser.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: The semantic analyser of the compiler.
  
 ==========================================================
 History:
 - 2016/11/03 09:52 : File created. Zhao Shixuan
***********************************************************/

using System.Collections.Generic;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public enum DomainType
    {
        TOP,
        CLASS,
        STATEMENT_BLOCK
    }
    
    public partial class SemanticAnalyser
    {
        /// <summary>
        /// Original code stream
        /// </summary>
        private List<IntermediateCode> SeCodeStream;

        /// <summary>
        /// Pointer points to the position of the current intermediate code in the SeCodeStream
        /// </summary>
        private int SePositionNow = 0;

        /// <summary>
        /// Symbol table of the semantic analyser
        /// </summary>
        private SymbolTable SeSymbolTable = new SymbolTable();

        /// <summary>
        /// Create a new semantic analyser
        /// </summary>
        /// <param name="CodeStream">Code Stream</param>
        /// <param name="DomainType">Type of Current Domain</param>
        /// <param name="AncestorSymbolTable">The Symbol Table of the Ancestor Domain</param>
        public SemanticAnalyser(List<IntermediateCode> CodeStream, DomainType DomainType, SymbolTable AncestorSymbolTable)
        {
            SeCodeStream = CodeStream;
            SeDomainType = DomainType;
            SeSymbolTable.Ancestor = AncestorSymbolTable;
        }

        /// <summary>
        /// Analyse semantic
        /// </summary>
        /// <returns>Analysed code</returns>
        public Domain SeAnalyse()
        {
            return SeAnalysedW();
        }

        public DomainType SeDomainType;

        /// <summary>
        /// Analyser worker
        /// </summary>
        /// <returns>Analysed code</returns>
        private Domain SeAnalysedW()
        {
            Domain CurrentDomain = new Domain();

            // Prescan the declarations when domain type is class
            if (SeDomainType == DomainType.CLASS)
            {
                int PrescanPositionNow = 0;
                for(; PrescanPositionNow < SeCodeStream.Count; PrescanPositionNow++)
                {
                    if(SeCodeStream[PrescanPositionNow].Type == ICStatementType.VARIABLE_DECLARATION)
                    {

                    }
                }
            }

            // Scan the code
            for (; SePositionNow < SeCodeStream.Count; SePositionNow++)
            {
                switch (SeCodeStream[SePositionNow].Type)
                {
                    case ICStatementType.VARIABLE_DECLARATION:
                        switch (SeDomainType)
                        {
                            case DomainType.TOP:

                                break;
                            case DomainType.CLASS:
                                // Do nothing. Scanned
                                break;
                            case DomainType.STATEMENT_BLOCK:
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }

            CurrentDomain.Code = SeCodeStream;
            CurrentDomain.Symbols = SeSymbolTable;
            return CurrentDomain;
        }

    }
}