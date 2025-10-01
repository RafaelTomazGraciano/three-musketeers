using System;
using System.Collections.Generic;

namespace Three_Musketeers.Models{
    public class SymbolTable{
        private Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>();

        public bool AddSymbol(Symbol symbol){
            if(symbolTable.ContainsKey(symbol.name)){
                return false;
            }

            symbolTable[symbol.name] = symbol;
            return true;
        }

        public Symbol? GetSymbol(string name){
            return symbolTable.TryGetValue(name, out var symbol) ? symbol : null;
        }

        public bool Contains(string name){
            return symbolTable.ContainsKey(name);
        }

        public void MarkInitializated(string name){
            if(symbolTable.TryGetValue(name, out var symbol)){
                symbol.isInitializated = true;
            }
        }
    }
}