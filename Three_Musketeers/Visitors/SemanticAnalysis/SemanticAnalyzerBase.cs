using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public abstract class SemanticAnalyzerBase : ExprBaseVisitor<string?>
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

            if (expr is ExprParser.CharLiteralContext)
            {
                return "char";
            }

            if (expr is ExprParser.TrueLiteralContext || expr is ExprParser.FalseLiteralContext)
            {
                return "bool";
            }

            if (expr is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                var symbol = symbolTable.GetSymbol(varName);
                return symbol?.type ?? "int";
            }

            if (expr is ExprParser.AddSubContext addSubCtx)
            {
                var leftType = GetExpressionType(addSubCtx.expr(0));
                var rightType = GetExpressionType(addSubCtx.expr(1));
                return PromoteTypes(leftType, rightType);
            }

            if (expr is ExprParser.MulDivModContext mulDivModCtx)
            {
                // Check if it's modulo operation
                string op = mulDivModCtx.GetChild(1).GetText();
                if (op == "%")
                {
                    return "int"; // Modulo always returns int
                }
                
                var leftType = GetExpressionType(mulDivModCtx.expr(0));
                var rightType = GetExpressionType(mulDivModCtx.expr(1));
                return PromoteTypes(leftType, rightType);
            }
            
            if (expr is ExprParser.UnaryMinusContext unaryMinusCtx)
            {
                return GetExpressionType(unaryMinusCtx.expr());
            }
            return "int";
        }

        private string PromoteTypes(string type1, string type2)
        {
            // If either is double, result is double
            if (type1 == "double" || type2 == "double")
                return "double";
            
            // Otherwise, result is int
            return "int";
        }
    }
}

