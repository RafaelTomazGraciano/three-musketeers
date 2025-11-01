using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.IncrementDecrement
{
    public class IncrementDecrementCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<ExprParser.ExprContext, string> visitExpresion;
        private readonly Func<ExprParser.IndexContext[], string> calculateArrayPosition;
        public IncrementDecrementCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Dictionary<string, Variable> variables,
            Func<ExprParser.ExprContext, string> visitExpresion,
            Func<ExprParser.IndexContext[], string> calculateArrayPosition
            )
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.variables = variables;
            this.visitExpresion = visitExpresion;
            this.calculateArrayPosition = calculateArrayPosition;
        }

        // Prefix increment: ++x
        public string VisitPrefixIncrement([NotNull] ExprParser.PrefixIncrementContext context)
        {
            string varName = context.ID().GetText();
            Variable variable = variables[varName];
            
            // Load current value
            string currentValue = LoadVariableValue(variable.register, variable.LLVMType);
            
            // Add 1
            string resultValue = PerformIncrementDecrement(currentValue, variable.LLVMType, true);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {variable.LLVMType} {resultValue}, {variable.LLVMType}* {variable.register}, align {GetAlignment(variable.LLVMType)}");
            
            // Return new value (prefix behavior)
            registerTypes[resultValue] = variable.LLVMType;
            return resultValue;
        }

        // Prefix decrement: --x
        public string VisitPrefixDecrement([NotNull] ExprParser.PrefixDecrementContext context)
        {
            string varName = context.ID().GetText();
            Variable variable = variables[varName];
            
            // Load current value
            string currentValue = LoadVariableValue(variable.register, variable.LLVMType);
            
            // Subtract 1
            string resultValue = PerformIncrementDecrement(currentValue, variable.LLVMType, false);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {variable.LLVMType} {resultValue}, {variable.LLVMType}* {variable.register}, align {GetAlignment(variable.LLVMType)}");
            
            // Return new value (prefix behavior)
            registerTypes[resultValue] = variable.LLVMType;
            return resultValue;
        }

        // Postfix increment: x++
        public string VisitPostfixIncrement([NotNull] ExprParser.PostfixIncrementContext context)
        {
            string varName = context.ID().GetText();
            Variable variable = variables[varName];
            
            // Load current value
            string currentValue = LoadVariableValue(variable.register, variable.LLVMType);
            
            // Store current value for return (postfix behavior) - just copy the value
            string returnValue = nextRegister();
            if (variable.LLVMType == "double")
            {
                getCurrentBody().AppendLine($"  {returnValue} = fadd double {currentValue}, 0.0");
            }
            else if (variable.LLVMType == "i8")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i8 {currentValue}, 0");
            }
            else if (variable.LLVMType == "i1")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i1 {currentValue}, 0");
            }
            else
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i32 {currentValue}, 0");
            }
            registerTypes[returnValue] = variable.LLVMType;
            
            // Add 1
            string resultValue = PerformIncrementDecrement(currentValue, variable.LLVMType, true);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {variable.LLVMType} {resultValue}, {variable.LLVMType}* {variable.register}, align {GetAlignment(variable.LLVMType)}");
            
            return returnValue;
        }

        // Postfix decrement: x--
        public string VisitPostfixDecrement([NotNull] ExprParser.PostfixDecrementContext context)
        {
            string varName = context.ID().GetText();
            Variable variable = variables[varName];
            
            // Load current value
            string currentValue = LoadVariableValue(variable.register, variable.LLVMType);
            
            // Store current value for return (postfix behavior) - just copy the value
            string returnValue = nextRegister();
            if (variable.LLVMType == "double")
            {
                getCurrentBody().AppendLine($"  {returnValue} = fadd double {currentValue}, 0.0");
            }
            else if (variable.LLVMType == "i8")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i8 {currentValue}, 0");
            }
            else if (variable.LLVMType == "i1")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i1 {currentValue}, 0");
            }
            else
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i32 {currentValue}, 0");
            }
            registerTypes[returnValue] = variable.LLVMType;
            
            // Subtract 1
            string resultValue = PerformIncrementDecrement(currentValue, variable.LLVMType, false);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {variable.LLVMType} {resultValue}, {variable.LLVMType}* {variable.register}, align {GetAlignment(variable.LLVMType)}");
            
            return returnValue;
        }

        // Prefix increment array: ++arr[0]
        public string VisitPrefixIncrementArray([NotNull] ExprParser.PrefixIncrementArrayContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variables[varName];
            
            // Calculate position
            string pos = calculateArrayPosition(indexes);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"  {elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, {arrayVar.innerType} 0, {arrayVar.innerType} {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Add 1
            string resultValue = PerformIncrementDecrement(currentValue, arrayVar.innerType, true);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            // Return new value (prefix behavior)
            registerTypes[resultValue] = arrayVar.innerType;
            return resultValue;
        }

        // Prefix decrement array: --arr[0]
        public string VisitPrefixDecrementArray([NotNull] ExprParser.PrefixDecrementArrayContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variables[varName];
            
            // Calculate position
            string pos = calculateArrayPosition(indexes);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"{elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, i32 0, {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Subtract 1
            string resultValue = PerformIncrementDecrement(currentValue, arrayVar.innerType, false);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            // Return new value (prefix behavior)
            registerTypes[resultValue] = arrayVar.innerType;
            return resultValue;
        }

        // Postfix increment array: arr[0]++
        public string VisitPostfixIncrementArray([NotNull] ExprParser.PostfixIncrementArrayContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variables[varName];
            
            // Calculate position
            string pos = calculateArrayPosition(indexes);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"{elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, i32 0, {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Store current value for return (postfix behavior) - just copy the value
            string returnValue = nextRegister();
            if (arrayVar.innerType == "double")
            {
                getCurrentBody().AppendLine($"  {returnValue} = fadd double {currentValue}, 0.0");
            }
            else if (arrayVar.innerType == "i8")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i8 {currentValue}, 0");
            }
            else if (arrayVar.innerType == "i1")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i1 {currentValue}, 0");
            }
            else
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i32 {currentValue}, 0");
            }
            registerTypes[returnValue] = arrayVar.innerType;
            
            // Add 1
            string resultValue = PerformIncrementDecrement(currentValue, arrayVar.innerType, true);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            return returnValue;
        }

        // Postfix decrement array: arr[0]--
        public string VisitPostfixDecrementArray([NotNull] ExprParser.PostfixDecrementArrayContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            ArrayVariable arrayVar = (ArrayVariable)variables[varName];
            
            // Calculate position
            string pos = calculateArrayPosition(indexes);
            
            // Get element pointer
            string elementPtr = nextRegister();
            getCurrentBody().AppendLine($"  {elementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, i32 0, {pos}");
            
            // Load current value
            string currentValue = LoadArrayElementValue(elementPtr, arrayVar.innerType);
            
            // Store current value for return (postfix behavior) - just copy the value
            string returnValue = nextRegister();
            if (arrayVar.innerType == "double")
            {
                getCurrentBody().AppendLine($"  {returnValue} = fadd double {currentValue}, 0.0");
            }
            else if (arrayVar.innerType == "i8")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i8 {currentValue}, 0");
            }
            else if (arrayVar.innerType == "i1")
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i1 {currentValue}, 0");
            }
            else
            {
                getCurrentBody().AppendLine($"  {returnValue} = add i32 {currentValue}, 0");
            }
            registerTypes[returnValue] = arrayVar.innerType;
            
            // Subtract 1
            string resultValue = PerformIncrementDecrement(currentValue, arrayVar.innerType, false);
            
            // Store new value
            getCurrentBody().AppendLine($"  store {arrayVar.innerType} {resultValue}, {arrayVar.innerType}* {elementPtr}, align {GetAlignment(arrayVar.innerType)}");
            
            return returnValue;
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

        private string PerformIncrementDecrement(string value, string llvmType, bool isIncrement)
        {
            string resultReg = nextRegister();
            
            if (llvmType == "double")
            {
                string oneValue = isIncrement ? "1.0" : "-1.0";
                string llvmOp = isIncrement ? "fadd" : "fsub";
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} double {value}, {oneValue}");
            }
            else if (llvmType == "i8")
            {
                string oneValue = isIncrement ? "1" : "-1";
                string llvmOp = isIncrement ? "add" : "sub";
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i8 {value}, {oneValue}");
            }
            else if (llvmType == "i1")
            {
                string oneValue = isIncrement ? "1" : "-1";
                string llvmOp = isIncrement ? "add" : "sub";
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i1 {value}, {oneValue}");
            }
            else
            {
                string oneValue = isIncrement ? "1" : "-1";
                string llvmOp = isIncrement ? "add" : "sub";
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i32 {value}, {oneValue}");
            }
            
            registerTypes[resultReg] = llvmType;
            return resultReg;
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
