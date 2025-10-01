using Antlr4.Runtime.Misc;
using System;
using System.Linq;
using Three_Musketeers.Models;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors{
    public class SemanticAnalyzer : ExprBaseVisitor<object?>{
        
        private SymbolTable symbolTable =  new SymbolTable();
        public bool hasErrors {get; private set;} = false;

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

            //int expectedArgs = CountFormatSpecifiers(formatString);
            var specifiers = ParseFormatSpecifiers(formatString);
            int providedArgs = context.expr()?.Length ?? 0;

            if (specifiers.Count != providedArgs) {
                ReportError(context.Start.Line,
                $"printf expects {specifiers.Count} argument(s) for '{formatString}', but recieved {providedArgs}");
                return null;
            }

            if(context.expr() != null){
                for (int i = 0; i < context.expr().Length; i++)
                {
                    var expr = context.expr()[i];
                    Visit(expr);

                    string exprType = GetExpressionType(expr);
                    char specifier = specifiers[i].type;

                    if (!TypesAreCompatible(exprType, specifier))
                    {
                        bool canConvert = (exprType == "int" && "f".Contains(specifier)) ||
                        (exprType == "double" && "di".Contains(specifier));

                        if (canConvert && exprType == "double" && "di".Contains(specifier))
                        {
                            ReportWarning(context.Start.Line,
                            $"Argument {i + 1}: implicit conversion from '{exprType}' to 'int' for '%%{specifier}' may lose precision");
                        }
                        else if (!canConvert)
                        {
                            ReportError(context.Start.Line,
                            $"Argument {i + 1}: format specifier '%%{specifier}' expects compatible type, but got '{exprType}'");
                        }
                    }
                }
            }
            return null;
        }
        
        private List<FormatSpecifier> ParseFormatSpecifiers(string formatString)
        {
            var specifiers = new List<FormatSpecifier>();
            
            for (int i = 0; i < formatString.Length; i++)
            {
                if (formatString[i] == '%')
                {
                    if (i + 1 >= formatString.Length)
                        break;
                        
                    if (formatString[i + 1] == '%')
                    {
                        i++;
                        continue;
                    }
                    
                    i++;
                    
                    while (i < formatString.Length && "+-0 #".Contains(formatString[i]))
                        i++;
                    while (i < formatString.Length && char.IsDigit(formatString[i]))
                        i++;
                    
                    int? precision = null;
                    if (i < formatString.Length && formatString[i] == '.')
                    {
                        i++;
                        int precisionValue = 0;
                        while (i < formatString.Length && char.IsDigit(formatString[i]))
                        {
                            precisionValue = precisionValue * 10 + (formatString[i] - '0');
                            i++;
                        }
                        precision = precisionValue;
                    }
                    
                    while (i < formatString.Length && "hlLzjt".Contains(formatString[i]))
                        i++;
                    
                    if (i < formatString.Length)
                    {
                        char type = formatString[i];
                        specifiers.Add(new FormatSpecifier(type, precision));
                    }
                }
            }
            
            return specifiers;
        }

        private string GetExpressionType(ExprParser.ExprContext expr)
        {
            if (expr is ExprParser.IntLiteralContext)
            {
                return "int";
            }
            else if (expr is ExprParser.DoubleLiteralContext)
            {
                return "double";
            }
            else if (expr is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                var symbol = symbolTable.GetSymbol(varName);
                return symbol?.type ?? "int";
            }
            else if (expr is ExprParser.AddSubContext || expr is ExprParser.MulDivContext)
            {
                return "int";
            }
            else if (expr is ExprParser.StringLiteralContext)
            {
                return "string";
            }
            
            return "int";
        }

        private bool TypesAreCompatible(string exprType, char specifier)
        {
            return specifier switch
            {
                'd' or 'i'  => exprType == "int",
                'f'  => exprType == "double" || exprType == "float",
                'c' => exprType == "int" || exprType == "char",
                's' => exprType == "string",
                _ => false
            };
        }

        public override object? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            string formatString = context.STRING_LITERAL().GetText();
            formatString = formatString.Substring(1, formatString.Length - 2); // remove quotes

            int expectedArgs = CountFormatSpecifiers(formatString);
            int providedArgs = context.expr()?.Length ?? 0;

            if (expectedArgs != providedArgs)
            {
                ReportError(context.Start.Line, $"scanf expects {expectedArgs} argument(s), but recieved {providedArgs}");
            }

            if (context.expr() != null)
            {
                foreach (var expr in context.expr())
                {
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
