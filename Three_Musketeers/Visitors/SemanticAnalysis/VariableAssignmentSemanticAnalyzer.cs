using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public class VariableAssignmentSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public VariableAssignmentSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.visitExpression = visitExpression;
        }

        public string? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            var typeToken = context.type();
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            string evalluatedType = visitExpression(context.expr());

            if (typeToken == null)
            {
                var existingSymbol = symbolTable.GetSymbol(varName);
                if (existingSymbol == null)
                {

                    reportError(context.Start.Line, $"Variable {varName} does not have a type");
                    return null;
                }
                existingSymbol.isInitializated = true;
                return existingSymbol.type;
            }

            string type = typeToken.GetText();

            if (symbolTable.Contains(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            var symbol = new Symbol(varName, type, line);
            symbolTable.AddSymbol(symbol);
            symbolTable.MarkInitializated(varName);
            return null;
        }

        public string? VisitDeclaration([NotNull] ExprParser.DeclarationContext context)
        {
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            if (symbolTable.Contains(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            var symbol = new Symbol(varName, type, line);
            symbolTable.AddSymbol(symbol);

            return type;
        }

        public string? VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            var symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            if (!symbol.isInitializated)
            {
                reportError(line, $"Variable '{varName}' is empty");
            }

            return symbol.type;
        }
        private static bool TwoTypesArePermitedToCast(string type1, string type2) {
            bool anyIsDouble = type1 == "double" || type2 == "double";
            bool anyIsChar = type1 == "char" || type2 == "char";
            bool anyIsInt = type1 == "int" || type2 == "int";
            bool anyIsBool = type1 == "bool" || type2 == "bool";
            bool anyIsString = type1 == "string" || type2 == "string";
            if (type1 == type2) return true;

            if (anyIsDouble && (anyIsChar || anyIsInt || anyIsBool)) return true;

            if (anyIsInt && (anyIsChar || anyIsBool)) return true;

            if (anyIsChar && (anyIsBool || !anyIsString)) return true;

            return false;
        }
    }
}

