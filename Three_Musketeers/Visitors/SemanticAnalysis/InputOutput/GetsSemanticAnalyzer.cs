using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.SemanticAnalysis.Struct_Unions;

namespace Three_Musketeers.Visitors.SemanticAnalysis.InputOutput
{
    public class GetsSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly LibraryDependencyTracker libraryTracker;
        private readonly StructSemanticAnalyzer structSemanticAnalyzer;
        
        public GetsSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable,
            LibraryDependencyTracker libraryTracker,
            StructSemanticAnalyzer structSemanticAnalyzer)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
            this.libraryTracker = libraryTracker;
            this.structSemanticAnalyzer = structSemanticAnalyzer;
        }

        public string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("gets", context.Start.Line))
            {
                return null;
            }

            // Struct/Union member access
            if (context.structGet() != null)
            {
                var structGetCtx = context.structGet();
                string varName = structGetCtx.ID().GetText();
                
                Symbol? symbol = symbolTable.GetSymbol(varName);
                if (symbol == null)
                {
                    reportError(context.Start.Line, $"Variable '{varName}' not declared before use in gets()");
                    return null;
                }

                if (symbol.isConstant)
                {
                    reportError(context.Start.Line, $"Cannot use #define constant '{varName}' in gets(). Constants are read-only");
                    return null;
                }

                // Use the struct analyzer to validate and get the final member type
                string? finalType = structSemanticAnalyzer.VisitStructGet(structGetCtx)!;
    
                // Clean up type name if it has "struct_" prefix
                if (finalType.StartsWith("struct_"))
                {
                    finalType = finalType.Substring(7);
                }

                if (finalType != "string")
                {
                    reportError(context.Start.Line,
                        $"gets() can only be used with string members, but this member is '{finalType}'");
                    return null;
                }

                // Mark the base variable as initialized
                symbolTable.MarkInitializated(varName);
                return null;
            }

            // Simple variable
            string varName2 = context.ID().GetText();

            Symbol? symbol2 = symbolTable.GetSymbol(varName2);
            if (symbol2 == null)
            {
                reportError(context.Start.Line,
                    $"Variable '{varName2}' not declared before use in gets()");
                return null;
            }

            if (symbol2.isConstant)
            {
                reportError(context.Start.Line,
                    $"Cannot use #define constant '{varName2}' in gets(). Constants are read-only");
                return null;
            }

            if (symbol2.type != "string")
            {
                reportError(context.Start.Line,
                    $"gets() can only be used with string variables, but '{varName2}' is '{symbol2.type}'");
                return null;
            }

            symbolTable.MarkInitializated(varName2);

            return null;
        }
    }
}