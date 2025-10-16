using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.CompoundAssignment
{
    public class CompoundAssignmentCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly VariableResolver variableResolver;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public CompoundAssignmentCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            VariableResolver variableResolver,
            Func<ExprParser.ExprContext, string> visitExpression)
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.variableResolver = variableResolver;
            this.visitExpression = visitExpression;
        }

        // Array element +=
        public string? VisitSingleAttPlusEquals([NotNull] ExprParser.SingleAttPlusEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variableResolver.GetVariable(varName)!;
            
            // Calculate position
            int pos = CalculateArrayPosition(indexes, arrayVar);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"  {elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, {arrayVar.innerType} 0, {arrayVar.innerType} {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Evaluate expression
            string exprValue = visitExpression(context.expr());
            
            // Perform addition
            string resultValue = PerformCompoundOperation(currentValue, exprValue, "+=", arrayVar.innerType);
            
            // Store result
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            return null;
        }

        // Array element -=
        public string? VisitSingleAttMinusEquals([NotNull] ExprParser.SingleAttMinusEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variableResolver.GetVariable(varName)!;
            
            // Calculate position
            int pos = CalculateArrayPosition(indexes, arrayVar);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"  {elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, {arrayVar.innerType} 0, {arrayVar.innerType} {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Evaluate expression
            string exprValue = visitExpression(context.expr());
            
            // Perform subtraction
            string resultValue = PerformCompoundOperation(currentValue, exprValue, "-=", arrayVar.innerType);
            
            // Store result
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            return null;
        }

        // Array element *=
        public string? VisitSingleAttMultiplyEquals([NotNull] ExprParser.SingleAttMultiplyEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variableResolver.GetVariable(varName)!;
            
            // Calculate position
            int pos = CalculateArrayPosition(indexes, arrayVar);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"  {elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, {arrayVar.innerType} 0, {arrayVar.innerType} {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Evaluate expression
            string exprValue = visitExpression(context.expr());
            
            // Perform multiplication
            string resultValue = PerformCompoundOperation(currentValue, exprValue, "*=", arrayVar.innerType);
            
            // Store result
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            return null;
        }

        // Array element /=
        public string? VisitSingleAttDivideEquals([NotNull] ExprParser.SingleAttDivideEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variableResolver.GetVariable(varName)!;
            
            // Calculate position
            int pos = CalculateArrayPosition(indexes, arrayVar);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"  {elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, {arrayVar.innerType} 0, {arrayVar.innerType} {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Evaluate expression
            string exprValue = visitExpression(context.expr());
            
            // Perform division
            string resultValue = PerformCompoundOperation(currentValue, exprValue, "/=", arrayVar.innerType);
            
            // Store result
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            return null;
        }

        private string LoadVariableValue(string register, string llvmType)
        {
            string loadReg = nextRegister();
            getCurrentBody().AppendLine($"  {loadReg} = load {llvmType}, {llvmType}* {register}, align {GetAlignment(llvmType)}");
            registerTypes[loadReg] = llvmType;
            return loadReg;
        }

        private string LoadArrayElementValue(string elementPtr, string llvmType)
        {
            string loadReg = nextRegister();
            getCurrentBody().AppendLine($"  {loadReg} = load {llvmType}, {llvmType}* {elementPtr}, align {GetAlignment(llvmType)}");
            registerTypes[loadReg] = llvmType;
            return loadReg;
        }

        private int CalculateArrayPosition(ExprParser.IndexContext[] indexes, ArrayVariable arrayVar)
        {
            int pos = 0;
            int i = 0;
            foreach (var dim in arrayVar.dimensions)
            {
                int index = int.Parse(indexes[i].INT().GetText());
                pos = pos * dim + index;
                i++;
            }
            return pos;
        }

        private string PerformCompoundOperation(string currentValue, string exprValue, string operation, string llvmType)
        {
            string exprType = registerTypes[exprValue];
            
            // Convert types if needed
            string convertedExpr = ConvertToType(exprValue, exprType, llvmType);
            
            string resultReg = nextRegister();
            
            if (llvmType == "double")
            {
                string llvmOp = operation switch
                {
                    "+=" => "fadd",
                    "-=" => "fsub",
                    "*=" => "fmul",
                    "/=" => "fdiv",
                    _ => "fadd"
                };
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} double {currentValue}, {convertedExpr}");
            }
            else if (llvmType == "i8")
            {
                string llvmOp = operation switch
                {
                    "+=" => "add",
                    "-=" => "sub",
                    "*=" => "mul",
                    "/=" => "sdiv",
                    _ => "add"
                };
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i8 {currentValue}, {convertedExpr}");
            }
            else if (llvmType == "i1")
            {
                string llvmOp = operation switch
                {
                    "+=" => "add",
                    "-=" => "sub",
                    "*=" => "mul",
                    "/=" => "sdiv",
                    _ => "add"
                };
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i1 {currentValue}, {convertedExpr}");
            }
            else
            {
                string llvmOp = operation switch
                {
                    "+=" => "add",
                    "-=" => "sub",
                    "*=" => "mul",
                    "/=" => "sdiv",
                    _ => "add"
                };
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i32 {currentValue}, {convertedExpr}");
            }
            
            registerTypes[resultReg] = llvmType;
            return resultReg;
        }

        private string ConvertToType(string value, string fromType, string toType)
        {
            if (fromType == toType) return value;
            
            string convReg = nextRegister();
            
            if (toType == "double")
            {
                if (fromType == "i32")
                {
                    getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {value} to double");
                }
                else if (fromType == "i8" || fromType == "i1")
                {
                    string intReg = nextRegister();
                    getCurrentBody().AppendLine($"  {intReg} = zext {fromType} {value} to i32");
                    getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {intReg} to double");
                }
            }
            else if (toType == "i32")
            {
                if (fromType == "double")
                {
                    getCurrentBody().AppendLine($"  {convReg} = fptosi double {value} to i32");
                }
                else if (fromType == "i8" || fromType == "i1")
                {
                    getCurrentBody().AppendLine($"  {convReg} = zext {fromType} {value} to i32");
                }
            }
            else if (toType == "i8" || toType == "i1")
            {
                if (fromType == "double")
                {
                    string intReg = nextRegister();
                    getCurrentBody().AppendLine($"  {intReg} = fptosi double {value} to i32");
                    getCurrentBody().AppendLine($"  {convReg} = trunc i32 {intReg} to {toType}");
                }
                else if (fromType == "i32")
                {
                    getCurrentBody().AppendLine($"  {convReg} = trunc i32 {value} to {toType}");
                }
            }
            
            registerTypes[convReg] = toType;
            return convReg;
        }

        private static int GetAlignment(string type)
        {
            return type switch
            {
                "i32" or "i32*" => 4,
                "double" => 8,
                "i1" or "i1*" or "i8" or "i8*" => 1,
                _ => 4
            };
        }
    }
}
