using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Visitors.SemanticAnalysis.Struct;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public abstract class SemanticAnalyzerBase : ExprBaseVisitor<string?>
    {
        protected SymbolTable symbolTable = new SymbolTable();
        protected Dictionary<string, FunctionInfo> declaredFunctions = new Dictionary<string, FunctionInfo>();
        protected Dictionary<string, HeterogenousInfo> structures = [];

        protected StructSemanticAnalyzer structSemanticAnalyzer;

        protected LibraryDependencyTracker libraryTracker;
        public bool hasErrors { get; protected set; } = false;

        public SemanticAnalyzerBase()
        {
            libraryTracker = new LibraryDependencyTracker(ReportError);
            structSemanticAnalyzer = new StructSemanticAnalyzer(symbolTable, structures, ReportError);
        }

        public override string? VisitStart([NotNull] ExprParser.StartContext context)
        {
            var includes = context.include();
            foreach (var include in includes)
            {
                Visit(include);
            }
    
            // Process all #define 
            var defines = context.define();
            foreach (var define in defines)
            {
                Visit(define);
            }
            
            CollectHeterogenousType(context);
            CollectFunctionSignatures(context);

            // global declarations
            var allProgs = context.prog();
            foreach (var prog in allProgs)
            {
                // Skip functions - they'll be processed later
                if (prog.function() != null)
                {
                    continue;
                }

                // Visit declarations and assignments
                Visit(prog);
            }

            // Visit functions
            foreach (var prog in allProgs)
            {
                if (prog.function() != null)
                {
                    Visit(prog.function());
                }
            }

            //visit main
            if (context.mainFunction() != null)
            {
                Visit(context.mainFunction());
            }

            return null;
        }
        
        public override string? VisitProg([NotNull] ExprParser.ProgContext context)
        {
            if (context.declaration() != null)
            {
                return Visit(context.declaration());
            }
            
            if (context.att() != null)
            {
                return Visit(context.att());
            }
            
            if (context.stm() != null)
            {
                return Visit(context.stm());
            }
            
            // Functions are handled separately in VisitStart
            return null;
        }

        private void CollectHeterogenousType(ExprParser.StartContext context)
        {
            var allProgs = context.prog();
            var heterogeneousContext = allProgs.Where(p => p.heteregeneousDeclaration() != null).Select(p => p.heteregeneousDeclaration());
            foreach (var prog in heterogeneousContext)
            {
                if (prog.structStatement() != null)
                {
                    structSemanticAnalyzer.VisitStructStatement(prog.structStatement());
                    continue;
                }

                if (prog.unionStatement() != null)
                {
                    structSemanticAnalyzer.VisitUnionStatement(prog.unionStatement());
                }
            }
        }

        private void CollectFunctionSignatures([NotNull] ExprParser.StartContext context)
        {
            var allProgs = context.prog();

            foreach (var prog in allProgs)
            {
                if (prog.function() != null)
                {
                    CollectSingleFunctionSignature(prog.function());
                }
            }
        }

        private void CollectSingleFunctionSignature(ExprParser.FunctionContext context)
        {
            int line = context.Start.Line;

            // Get return type
            var returnTypeCtx = context.function_return();
            string returnType = "";
            int returnPointerLevel = returnTypeCtx.POINTER()?.Length ?? 0;

            if (returnTypeCtx.VOID() != null)
            {
                returnType = "void";
            }
            else if (returnTypeCtx.type() != null)
            {
                returnType = GetTypeStringFromContext(returnTypeCtx.type());
            }
            else
            {
                ReportError(line, $"Invalid return type in function");
                return;
            }

            // Get function name
            string functionName = context.ID().GetText();

            // Check for duplicates
            if (declaredFunctions.ContainsKey(functionName))
            {
                ReportError(line, $"Function '{functionName}' has already been declared");
                return;
            }

            // Create function info
            var functionInfo = new FunctionInfo
            {
                returnType = returnType,
                returnPointerLevel = returnPointerLevel,
                parameters = new List<(string, string, int)>(),
                hasReturnStatement = false
            };

            // Process parameters
            var argsCtx = context.args();
            if (argsCtx != null)
            {
                var types = argsCtx.type();
                var ids = argsCtx.ID();
                var allPointers = argsCtx.POINTER();

                HashSet<string> paramNames = new HashSet<string>();
                int pointerIndex = 0;

                for (int i = 0; i < types.Length; i++)
                {
                    string paramType = GetTypeStringFromContext(types[i]);
                    string paramName = ids[i].GetText();

                    int pointerLevel = 0;
                    int typeEndPos = types[i].Stop.StopIndex;
                    int idStartPos = ids[i].Symbol.StartIndex;

                    while (pointerIndex < allPointers.Length && 
                    allPointers[pointerIndex].Symbol.StartIndex > typeEndPos &&
                    allPointers[pointerIndex].Symbol.StartIndex < idStartPos)
                    {
                        pointerLevel++;
                        pointerIndex++;
                    }
                    
                    if (paramNames.Contains(paramName))
                    {
                        ReportError(line, $"Parameter '{paramName}' duplicated in function '{functionName}'");
                        continue;
                    }
                    
                    paramNames.Add(paramName);
                    functionInfo.parameters?.Add((paramType, paramName, pointerLevel));
                }
            }

            // Register function
            declaredFunctions[functionName] = functionInfo;

            // Add function to symbol table as a special symbol
            var functionSymbol = new Symbol(functionName, returnType, line);
            functionSymbol.isInitializated = true;
            symbolTable.AddSymbol(functionSymbol);
        }

        private string GetTypeStringFromContext(ExprParser.TypeContext context)
        {
            if (context.GetText() == "int") return "int";
            if (context.GetText() == "double") return "double";
            if (context.GetText() == "bool") return "bool";
            if (context.GetText() == "char") return "char";
            if (context.GetText() == "string") return "string";

            var id = context.ID();
            if (id != null)
            {
                string typeName = id.GetText();
                structures.TryGetValue(typeName, out var structInfo);
                if (structInfo != null)
                {
                    return structInfo.name;
                }
                ReportError(context.Start.Line, $"type '{typeName}' has not been defined");
                return "error";
            }

            return "unknown";
        }

        protected void ReportError(int line, string message)
        {
            hasErrors = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[SEMANTIC ERROR] Line {line}");
            Console.WriteLine($"  {message}");
            Console.ResetColor();
        }

        protected static void ReportWarning(int line, string message)
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

            // String conversions
            if (expr is ExprParser.AtoiConversionContext)
            {
                return "int";
            }

            if (expr is ExprParser.AtodConversionContext)
            {
                return "double";
            }

            if (expr is ExprParser.ItoaConversionContext)
            {
                return "string";
            }

            if (expr is ExprParser.DtoaConversionContext)
            {
                return "string";
            }

            if (expr is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                var symbol = symbolTable.GetSymbol(varName);
                return symbol?.type ?? "int";
            }

            if (expr is ExprParser.VarStructContext varStructCtx)
            {
                string? structType = structSemanticAnalyzer.VisitStructGet(varStructCtx.structGet());
                return structType ?? "int";
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

            if (expr is ExprParser.LogicalAndOrContext logicalAndOrCtx)
            {
                return "bool";
            }

            if (expr is ExprParser.LogicalNotContext logicalNotCtx)
            {
                return "bool";
            }

            if (expr is ExprParser.EqualityContext equalityCtx)
            {
                return "bool";
            }

            if (expr is ExprParser.ComparisonContext comparisonCtx)
            {
                return "bool";
            }

            if (expr is ExprParser.FunctionCallContext funcCallCtx)
            {
                string funcName = funcCallCtx.ID().GetText();
                if (declaredFunctions.ContainsKey(funcName))
                {
                    return declaredFunctions[funcName].returnType ?? "int";
                }
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
