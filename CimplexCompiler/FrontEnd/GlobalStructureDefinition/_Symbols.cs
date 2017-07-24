/***********************************************************
 Nano Studio Source File
 Copyright (C) 2015 Nano Studio.
 ==========================================================
 Category: Internal - Cimplex
 ==========================================================
 File ID: NSKI1407141603
 Date of Generating: 2014/07/14
 File Name: _Symbols.cs
 NameSpaces: 1/1 NanoStudio.Internel.Cimplex.Compiler
  
 ==========================================================
 Abstract: Symbol definitions.
  
 ==========================================================
 History:
 - 2014/07/14 16:03 : File created. Zhao Shixuan
***********************************************************/

using System.Collections.Generic;

namespace NanoStudio.Internel.Cimplex.Compiler
{
    public struct PreDefinedSymbol
    {
        public string Name;
        public string Content;
    }

    public class Symbol
    {
        public string Name;
        public bool Solved = false;
        public int Type;
        public string TypeName;
        public bool IsMethod = false;
        public List<ICVariableDeclaration> ArgumentList = new List<ICVariableDeclaration>();
    }

    public class SymbolTable
    {
        public HashSet<Symbol> Table = new HashSet<Symbol>();
        public SymbolTable Ancestor = null;

        public bool AddSymbol(Symbol Item)
        {
            return Table.Add(Item);
        }
    }
}