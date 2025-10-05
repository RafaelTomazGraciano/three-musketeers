using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis.InputOutput
{
    public class ScanfSemanticAnalyzer
    {

        private readonly Action<int, string> reportError;
        private readonly SymbolTable symbolTable;

        public ScanfSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
        }

        public string? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            var ids = context.ID();

            if (ids.Length == 0)
            {
                reportError(context.Start.Line, "scanf requires at least one variable");
                return null;
            }

            foreach (var idToken in context.ID())
            {
                string varName = idToken.GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

                if (symbol == null)
                {
                    reportError(context.Start.Line,
                    $"Variable '{varName}' not declared before use in scanf");
                    continue;
                }

                if (symbol.type == "string")
                {
                    reportError(context.Start.Line,
                        $"scanf() cannot be used with string variables. Use gets() for '{varName}' instead");
                    continue;
                }

                symbolTable.MarkInitializated(varName);
            }
            return null;
        }
    }
}