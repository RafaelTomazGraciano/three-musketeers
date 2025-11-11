using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class ScanfCodeGenerator
    {
        private readonly StringBuilder globalStrings;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<string> nextRegister;
        private readonly Func<string> nextStringLabel;
        private readonly Func<string, string> getLLVMType;
        private readonly VariableResolver variableResolver;
        private readonly Func<ExprParser.IndexContext[], string> calculateArrayPosition;
        private readonly Func<ExprParser.StructGetContext, string> visitStructGet;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string, int> getAlignment;

        public ScanfCodeGenerator(
            StringBuilder globalStrings,
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, Variable> variables,
            Func<string> nextRegister,
            Func<string> nextStringLabel,
            Func<string, string> getLLVMType,
            VariableResolver variableResolver,
            Func<ExprParser.IndexContext[], string> calculateArrayPosition,
            Func<ExprParser.StructGetContext, string> visitStructGet,
            Dictionary<string, string> registerTypes,
            Func<string, int> getAlignment)
        {
            this.globalStrings = globalStrings;
            this.getCurrentBody = getCurrentBody;
            this.variables = variables;
            this.nextRegister = nextRegister;
            this.nextStringLabel = nextStringLabel;
            this.getLLVMType = getLLVMType;
            this.variableResolver = variableResolver;
            this.calculateArrayPosition = calculateArrayPosition;
            this.visitStructGet = visitStructGet;
            this.registerTypes = registerTypes;
            this.getAlignment = getAlignment;
        }

        public string? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            var args = context.scanfArg();
            if (args.Length == 0)
            {
                return null;
            }

            var formatSpecifiers = new List<string>();
            var variablePointers = new List<string>();
            var currentBody = getCurrentBody();

            foreach (var arg in args)
            {
                // Get pointer to the variable/member and its type
                var (pointerReg, llvmType, varType) = GetVariablePointer(arg);

                string argName = arg.ID()?.GetText() ?? "struct_member";

                if (pointerReg == null || llvmType == null || varType == null)
                {
                    string argText = arg.ID()?.GetText() ?? arg.structGet()?.GetText() ?? "unknown";
                    throw new Exception($"Failed to resolve scanf argument: {argText}");
                }

                // Get format specifier based on the variable type
                string formatSpec = GetFormatSpecifier(varType);
                formatSpecifiers.Add(formatSpec);

                // Add pointer to arguments list
                variablePointers.Add($"{llvmType}* {pointerReg}");
            }

            // Create format string
            string formatStr = string.Join(" ", formatSpecifiers);
            string strLabel = nextStringLabel();
            int strLen = formatStr.Length + 1;

            globalStrings.AppendLine($"{strLabel} = private unnamed_addr constant [{strLen} x i8] c\"{formatStr}\\00\", align 1");

            // Get pointer to format string
            string ptrReg = nextRegister();
            currentBody.AppendLine($"  {ptrReg} = getelementptr inbounds [{strLen} x i8], [{strLen} x i8]* {strLabel}, i32 0, i32 0");

            // Build scanf call with all arguments
            string resultReg = nextRegister();
            string allArgs = string.Join(", ", variablePointers);
            currentBody.AppendLine($"  {resultReg} = call i32 (i8*, ...) @scanf(i8* {ptrReg}, {allArgs})");

            return null;
        }

        private (string? pointerReg, string? llvmType, string? varType) GetVariablePointer(ExprParser.ScanfArgContext context)
        {
            var currentBody = getCurrentBody();

            // Struct/Union member access 
            if (context.structGet() != null)
            {
                var structGetCtx = context.structGet();
                
                // VisitStructGet returns a pointer to the member
                string memberPtrReg = visitStructGet(structGetCtx);
                
                if (memberPtrReg == null || !registerTypes.ContainsKey(memberPtrReg))
                {
                    return (null, null, null);
                }

                string llvmType = registerTypes[memberPtrReg];
                string varType = LLVMTypeToVarType(llvmType);

                return (memberPtrReg, llvmType, varType);
            }

            // Simple variable or array element 
            string varName = context.ID().GetText();
            Variable? variable = variableResolver.FindVariable(varName);

            if (variable == null)
            {
                return (null, null, null);
            }

            var indexes = context.index();

            // Simple variable 
            if (indexes.Length == 0)
            {
                string llvmType = variable.LLVMType;
                string varType = variable.type;

                return (variable.register, llvmType, varType);
            }

            // Array element access
            if (variable is ArrayVariable arrayVar)
            {
                string arrayPositions = calculateArrayPosition(indexes);
                string gepReg = nextRegister();
                string elementType = arrayVar.innerType;

                currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, i32 0, {arrayPositions}");

                string varType = LLVMTypeToVarType(elementType);
                return (gepReg, elementType, varType);
            }

            // Pointer indexing
            if (variable.LLVMType.Contains('*'))
            {
                // Load the pointer first
                string loadedPointer = nextRegister();
                currentBody.AppendLine($"  {loadedPointer} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align {getAlignment(variable.LLVMType)}");

                // Calculate position
                string arrayPositions = calculateArrayPosition(indexes);
                string elementType = variable.LLVMType.TrimEnd('*');
                
                string gepReg = nextRegister();
                currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {elementType}, {variable.LLVMType} {loadedPointer}, {arrayPositions}");

                string varType = LLVMTypeToVarType(elementType);
                return (gepReg, elementType, varType);
            }

            return (null, null, null);
        }

        private string GetFormatSpecifier(string type)
        {
            return type switch
            {
                "int" => "%d",
                "double" => "%lf",
                "char" => "%c",
                "bool" => "%d",
                _ => "%d"
            };
        }

        private string LLVMTypeToVarType(string llvmType)
        {
            // Remove array notation if present 
            string cleanType = llvmType;
            if (cleanType.Contains('['))
            {
                int lastX = cleanType.LastIndexOf(" x ");
                if (lastX != -1)
                {
                    cleanType = cleanType.Substring(lastX + 3).TrimEnd(']', ' ');
                }
            }

            return cleanType switch
            {
                "i32" => "int",
                "double" => "double",
                "i8" => "char",
                var t when t.StartsWith("%") => t.TrimStart('%'), // struct type
                _ => "int"
            };
        }
    }
}