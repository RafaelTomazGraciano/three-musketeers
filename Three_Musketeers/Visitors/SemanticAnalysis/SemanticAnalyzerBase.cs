using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public abstract class SemanticAnalyzerBase : ExprBaseVisitor<string>
    {
        protected SymbolTable symbolTable = new SymbolTable();
        public bool hasErrors { get; protected set; } = false;

        protected void ReportError(int line, string message)
        {
            hasErrors = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[SEMANTIC ERROR] Line {line}");
            Console.WriteLine($"  {message}");
            Console.ResetColor();
        }

        protected void ReportWarning(int line, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[WARNING] Line {line}");
            Console.WriteLine($"  {message}");
            Console.ResetColor();
        }

        protected string GetExpressionType(ExprParser.ExprContext expr)
        {
            if (expr is ExprParser.IntLiteralContext)
            {
                return "int";
            }

            if (expr is ExprParser.DoubleLiteralContext)
            {
                return "double";
            }

            if (expr is ExprParser.StringLiteralContext)
            {
                return "string";
            }

            if (expr is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                var symbol = symbolTable.GetSymbol(varName);
                return symbol?.type ?? "int";
            }

            if (expr is ExprParser.AddSubContext || expr is ExprParser.MulDivContext)
            {
                return "int";
            }
            
            return "char";
        }
    }
}

