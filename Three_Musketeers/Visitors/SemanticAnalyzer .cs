using Antlr4.Runtime.Misc;
using System;
using System.Linq;
using Three_Musketeers.Models;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors{
    public class SemanticAnalyzer : ExprBaseVisitor<object?>{
        private SymbolTable symbolTable =  new SymbolTable();
        public bool hasErrors {get; private set;} = false;

        //visit all
        public override object? VisitStart([NotNull] ExprParser.StartContext context){
            return base.VisitStart(context);
        }

        public override object? VisitProg ([NotNull] ExprParser.ProgContext context){
            return base.VisitProg(context);
        }

        public override object? VisitAtt([NotNull] ExprParser.AttContext context){
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            if(symbolTable.Contains(varName)){
                ReportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            var symbol = new Symbol(varName, type, line);
            symbolTable.AddSymbol(symbol);
            symbolTable.MarkInitializated(varName);

            Visit(context.expr());

            return null;
        }

        public override object? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context){
            string formatString = context.STRING_LITERAL().GetText();
            formatString = formatString.Substring(1, formatString.Length - 2); // remove quotes

            int expectedArgs = CountFormatSpecifiers(formatString);
            int providedArgs = context.expr()?.Length ?? 0;

            if(expectedArgs != providedArgs){
                ReportError(context.Start.Line, 
                $"printf expects {expectedArgs} argument(s) for '{formatString}', but recieved {providedArgs}");
            }

            if(context.expr() != null){
                foreach(var expr in context.expr()){
                    Visit(expr);
                }
            }
            return null;
        }

        public override object? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context){
            string formatString = context.STRING_LITERAL().GetText();
            formatString = formatString.Substring(1, formatString.Length - 2); // remove quotes

            int expectedArgs = CountFormatSpecifiers(formatString);
            int providedArgs = context.expr()?.Length ?? 0;

            if(expectedArgs != providedArgs){
                ReportError(context.Start.Line, $"scanf expects {expectedArgs} argument(s), but recieved {providedArgs}");
            }

            if(context.expr() != null){
                foreach(var expr in context.expr()){
                    Visit(expr);
                }
            }
            return null;
        }

        public override object? VisitVar([NotNull] ExprParser.VarContext context){
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            var symbol = symbolTable.GetSymbol(varName);
            if(symbol == null){
                ReportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            if(!symbol.isInitializated){
                ReportWarning(line, $"Variable '{varName}' may not be initialized");
            }

            return symbol.type;
        }

        private int CountFormatSpecifiers(string formatString){
            int count = 0;
            for(int i = 0; i < formatString.Length - 1; i++){
                if(formatString[i] == '%' && formatString[i+1] != '%'){
                    char next = formatString[i+1];
                    if("difcs".Contains(next)){
                        count++;
                    }
                }
            }
            return count;
        }

        private void ReportError(int line, string message){
            hasErrors = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[SEMANTIC ERROR] Line {line}");
            Console.WriteLine($"  {message}");
            Console.ResetColor();
        }

        private void ReportWarning(int line, string message){
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[WARNING] Line {line}");
            Console.WriteLine($"  {message}");
            Console.ResetColor();
        }

    }
}
