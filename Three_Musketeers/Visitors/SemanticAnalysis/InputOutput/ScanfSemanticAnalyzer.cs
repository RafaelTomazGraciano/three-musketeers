using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.SemanticAnalysis.Struct_Unions;

namespace Three_Musketeers.Visitors.SemanticAnalysis.InputOutput
{
    public class ScanfSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly SymbolTable symbolTable;
        private readonly LibraryDependencyTracker libraryTracker;
        private readonly StructSemanticAnalyzer structSemanticAnalyzer;

        public ScanfSemanticAnalyzer(
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

        public string? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("scanf", context.Start.Line))
            {
                return null;
            }
    
            var args = context.scanfArg();

            if (args.Length == 0)
            {
                reportError(context.Start.Line, "scanf requires at least one variable");
                return null;
            }

            foreach (var arg in args)
            {
                ValidateScanfArgument(arg);
            }
            
            return null;
        }

        private void ValidateScanfArgument(ExprParser.ScanfArgContext context)
        {
            int line = context.Start.Line;
            
            // struct/union member access
            if (context.structGet() != null)
            {
                var structGetCtx = context.structGet();
                string varName = structGetCtx.ID().GetText();
                
                Symbol? symbol = symbolTable.GetSymbol(varName);
                if (symbol == null)
                {
                    reportError(line, $"Variable '{varName}' not declared before use in scanf");
                    return;
                }

                if (symbol.isConstant)
                {
                    reportError(line, $"Cannot use #define constant '{varName}' in scanf(). Constants are read-only");
                    return;
                }

                // Use the struct analyzer to validate and get the final member type
                string? finalType = structSemanticAnalyzer.VisitStructGet(structGetCtx);
                
                if (finalType == null)
                {
                    // Error already reported by StructSemanticAnalyzer
                    return;
                }

                // Clean up type name if it has "struct_" prefix
                if (finalType.StartsWith("struct_"))
                {
                    finalType = finalType.Substring(7);
                }

                if (finalType == "string")
                {
                    reportError(line, "scanf() cannot be used with string variables. Use gets() instead");
                    return;
                }

                // Mark the base variable as initialized
                symbolTable.MarkInitializated(varName);
                return;
            }
            
            // simple variable or array element
            string id = context.ID().GetText();
            Symbol? varSymbol = symbolTable.GetSymbol(id);

            if (varSymbol == null)
            {
                reportError(line, $"Variable '{id}' not declared before use in scanf");
                return;
            }

            if (varSymbol.isConstant)
            {
                reportError(line, $"Cannot use #define constant '{id}' in scanf(). Constants are read-only");
                return;
            }

            // Determine the final type after array indexing
            string finalVarType = varSymbol.type;
            var indexes = context.index();
            
            // If it's an array access, validate and get the element type
            if (indexes.Length > 0)
            {
                if (varSymbol is ArraySymbol arraySymbol)
                {
                    if (indexes.Length > arraySymbol.dimensions.Count)
                    {
                        reportError(line, 
                            $"Too many indices for array '{id}'. Expected {arraySymbol.dimensions.Count}, got {indexes.Length}");
                        return;
                    }
                    
                    if (indexes.Length < arraySymbol.dimensions.Count)
                    {
                        reportError(line, 
                            $"Array '{id}' must be fully indexed in scanf. Expected {arraySymbol.dimensions.Count} indices, got {indexes.Length}");
                        return;
                    }
                    
                    finalVarType = arraySymbol.elementType;
                }
                else if (varSymbol is PointerSymbol pointerSymbol)
                {
                    // Pointer can be indexed once to access the pointee
                    if (indexes.Length > 1)
                    {
                        reportError(line, $"Too many indices for pointer '{id}'");
                        return;
                    }
                    finalVarType = pointerSymbol.pointeeType;
                }
                else
                {
                    reportError(line, $"Variable '{id}' is not an array or pointer but is being indexed");
                    return;
                }
            }

            // Clean up type name if it has "struct_" prefix
            if (finalVarType.StartsWith("struct_"))
            {
                reportError(line, $"Cannot use scanf() with struct type. Use scanf on individual struct members instead");
                return;
            }

            if (finalVarType == "string")
            {
                reportError(line, $"scanf() cannot be used with string variables. Use gets() for '{id}' instead");
                return;
            }

            // Mark variable as initialized
            symbolTable.MarkInitializated(id);
        }
    }
}