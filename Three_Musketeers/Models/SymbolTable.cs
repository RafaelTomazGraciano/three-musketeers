using System;
using System.Collections.Generic;

namespace Three_Musketeers.Models{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>();
        private Scope currentScope;
        private Scope globalScope;

        public SymbolTable()
        {
            globalScope = new Scope();
            currentScope = globalScope;
        }

        public void EnterScope()
        {
            currentScope = new Scope(currentScope);
        }

        public void ExitScope()
        {
            if (currentScope.parent != null)
            {
                currentScope = currentScope.parent;
            }
            else
            {
                throw new InvalidOperationException("Cannot exit global scope");
            }
        }

        public bool IsGlobalScope()
        {
            return currentScope == globalScope;
        }

        public bool AddSymbol(Symbol symbol)
        {
            if (currentScope.symbols.ContainsKey(symbol.name))
            {
                return false;
            }
            currentScope.symbols[symbol.name] = symbol;
            return true;
        }

        public Symbol? GetSymbol(string name)
        {
            Scope? scope = currentScope;
            while (scope != null)
            {
                if (scope.symbols.TryGetValue(name, out var symbol))
                {
                    return symbol;
                }
                scope = scope.parent;
            }
            return null;
        }

        public bool ContainsInCurrentScope(string name)
        {
            return currentScope.symbols.ContainsKey(name);
        }

        public bool Contains(string name)
        {
            return GetSymbol(name) != null;
        }

        public void MarkInitializated(string name)
        {
            Scope? scope = currentScope;
            while (scope != null)
            {
                if (scope.symbols.TryGetValue(name, out var symbol))
                {
                    symbol.isInitializated = true;
                    return;
                }
                scope = scope.parent;
            }
        }

        public Dictionary<string, Symbol> GetCurrentScopeSymbols()
        {
            return new Dictionary<string, Symbol>(currentScope.symbols);
        }

        public Dictionary<string, Symbol> GetAllSymbols()
        {
            var allSymbols = new Dictionary<string, Symbol>();
            Scope? scope = currentScope;

            while (scope != null)
            {
                foreach (var kvp in scope.symbols)
                {
                    if (!allSymbols.ContainsKey(kvp.Key))
                    {
                        allSymbols[kvp.Key] = kvp.Value;
                    }
                }
                scope = scope.parent;
            }

            return allSymbols;
        }
        
        public void Clear()
        {
            globalScope = new Scope();
            currentScope = globalScope;
        }
    }
}