using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.Arithmetic
{
    public class ArithmeticCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public ArithmeticCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string> visitExpression)
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
        }

        public string VisitAddSub([NotNull] ExprParser.AddSubContext context)
        {
            string leftValue = visitExpression(context.expr(0));
            string rightValue = visitExpression(context.expr(1));
            
            string leftType = registerTypes[leftValue];
            string rightType = registerTypes[rightValue];
            
            // Get the operation symbol
            string op = context.GetChild(1).GetText();
            
            // Check for pointer arithmetic
            bool leftIsPointer = leftType.Contains('*');
            bool rightIsPointer = rightType.Contains('*');
            
            // Handle pointer arithmetic
            if (leftIsPointer || rightIsPointer)
            {
                return HandlePointerArithmetic(leftValue, leftType, rightValue, rightType, op);
            }
            
            // Handle type promotion for non-pointer types
            string resultType = PromoteTypes(leftType, rightType);
            
            if (resultType == "double")
            {
                // Convert to double if needed - generate conversion instructions first
                string leftDouble = ConvertToDouble(leftValue, leftType);
                string rightDouble = ConvertToDouble(rightValue, rightType);
                
                string llvmOp = op switch
                {
                    "+" => "fadd",
                    "-" => "fsub",
                    _ => "fadd"
                };
                
                string resultReg = nextRegister();
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} double {leftDouble}, {rightDouble}");
                registerTypes[resultReg] = resultType;
                return resultReg;
            }
            else
            {
                // Integer operations
                string leftInt = ConvertToInt(leftValue, leftType);
                string rightInt = ConvertToInt(rightValue, rightType);
                
                string llvmOp = op switch
                {
                    "+" => "add",
                    "-" => "sub",
                    _ => "add"
                };
                
                string resultReg = nextRegister();
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i32 {leftInt}, {rightInt}");
                registerTypes[resultReg] = resultType;
                return resultReg;
            }
        }

        private string HandlePointerArithmetic(string leftValue, string leftType, string rightValue, string rightType, string op)
        {
            // Pointer arithmetic: ptr + int or ptr - int or ptr - ptr
            bool leftIsPointer = leftType.Contains('*');
            bool rightIsPointer = rightType.Contains('*');
            
            if (leftIsPointer && !rightIsPointer)
            {
                // ptr + int or ptr - int
                // Use getelementptr for pointer arithmetic
                string offset = rightValue;
                string offsetType = rightType;
                
                // Convert offset to i32 if needed
                if (offsetType != "i32")
                {
                    offset = ConvertToInt(offset, offsetType);
                }
                
                // For subtraction, negate the offset
                if (op == "-")
                {
                    string negReg = nextRegister();
                    getCurrentBody().AppendLine($"  {negReg} = sub i32 0, {offset}");
                    offset = negReg;
                }
                
                // Get the base type (remove one *)
                string baseType = leftType.Substring(0, leftType.Length - 1);
                
                string resultReg = nextRegister();
                getCurrentBody().AppendLine($"  {resultReg} = getelementptr inbounds {baseType}, {leftType} {leftValue}, i32 {offset}");
                registerTypes[resultReg] = leftType;
                return resultReg;
            }
            else if (!leftIsPointer && rightIsPointer)
            {
                // int + ptr (commutative for addition only)
                if (op == "+")
                {
                    string offset = leftValue;
                    string offsetType = leftType;
                    
                    // Convert offset to i32 if needed
                    if (offsetType != "i32")
                    {
                        offset = ConvertToInt(offset, offsetType);
                    }
                    
                    // Get the base type (remove one *)
                    string baseType = rightType.Substring(0, rightType.Length - 1);
                    
                    string resultReg = nextRegister();
                    getCurrentBody().AppendLine($"  {resultReg} = getelementptr inbounds {baseType}, {rightType} {rightValue}, i32 {offset}");
                    registerTypes[resultReg] = rightType;
                    return resultReg;
                }
                else
                {
                    throw new Exception("Invalid pointer arithmetic: cannot subtract pointer from integer");
                }
            }
            else if (leftIsPointer && rightIsPointer)
            {
                // ptr - ptr: result is the difference in elements (ptrdiff_t, i64)
                if (op == "-")
                {
                    // Convert both pointers to integers
                    string leftInt = nextRegister();
                    getCurrentBody().AppendLine($"  {leftInt} = ptrtoint {leftType} {leftValue} to i64");
                    
                    string rightInt = nextRegister();
                    getCurrentBody().AppendLine($"  {rightInt} = ptrtoint {rightType} {rightValue} to i64");
                    
                    // Subtract
                    string diffReg = nextRegister();
                    getCurrentBody().AppendLine($"  {diffReg} = sub i64 {leftInt}, {rightInt}");
                    
                    // Divide by element size to get number of elements
                    // Get the base type (remove one *)
                    string baseType = leftType.Substring(0, leftType.Length - 1);
                    int elementSize = GetElementSize(baseType);
                    
                    string resultReg = nextRegister();
                    getCurrentBody().AppendLine($"  {resultReg} = sdiv i64 {diffReg}, {elementSize}");
                    
                    // Convert result to i32
                    string finalReg = nextRegister();
                    getCurrentBody().AppendLine($"  {finalReg} = trunc i64 {resultReg} to i32");
                    
                    registerTypes[finalReg] = "i32";
                    return finalReg;
                }
                else
                {
                    throw new Exception("Invalid pointer arithmetic: cannot add two pointers");
                }
            }
            
            throw new Exception("Unexpected pointer arithmetic case");
        }
        
        private int GetElementSize(string type)
        {
            // Return size in bytes
            if (type.Contains('*')) return 8;
            if (type == "i32") return 4;
            if (type == "i8") return 1;
            if (type == "double") return 8;
            if (type == "i1") return 1;
            return 4;
        }

        public string VisitMulDivMod([NotNull] ExprParser.MulDivModContext context)
        {
            string leftValue = visitExpression(context.expr(0));
            string rightValue = visitExpression(context.expr(1));
            
            string leftType = registerTypes[leftValue];
            string rightType = registerTypes[rightValue];
            
            // Get the operation symbol
            string op = context.GetChild(1).GetText();
            
            if (op == "%")
            {
                // Modulo operation - only works with integers
                string leftInt = ConvertToInt(leftValue, leftType);
                string rightInt = ConvertToInt(rightValue, rightType);
                
                string resultReg = nextRegister();
                getCurrentBody().AppendLine($"  {resultReg} = srem i32 {leftInt}, {rightInt}");
                registerTypes[resultReg] = "i32";
                return resultReg;
            }
            
            // Handle multiplication and division
            string resultType = PromoteTypes(leftType, rightType);
            
            if (resultType == "double")
            {
                // Convert to double if needed - generate conversion instructions first
                string leftDouble = ConvertToDouble(leftValue, leftType);
                string rightDouble = ConvertToDouble(rightValue, rightType);
                
                string llvmOp = op switch
                {
                    "*" => "fmul",
                    "/" => "fdiv",
                    _ => "fmul"
                };
                
                string resultReg = nextRegister();
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} double {leftDouble}, {rightDouble}");
                registerTypes[resultReg] = resultType;
                return resultReg;
            }
            else
            {
                // Integer operations
                string leftInt = ConvertToInt(leftValue, leftType);
                string rightInt = ConvertToInt(rightValue, rightType);
                
                string llvmOp = op switch
                {
                    "*" => "mul",
                    "/" => "sdiv",
                    _ => "mul"
                };
                
                string resultReg = nextRegister();
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i32 {leftInt}, {rightInt}");
                registerTypes[resultReg] = resultType;
                return resultReg;
            }
        }

        private string PromoteTypes(string type1, string type2)
        {
            // If either is double, result is double
            if (type1 == "double" || type2 == "double")
                return "double";
            
            // Otherwise, result is int
            return "i32";
        }

        private string ConvertToDouble(string value, string currentType)
        {
            if (currentType == "double")
                return value;
            
            string convReg = nextRegister();
            if (currentType == "i32")
            {
                // Check if it's a literal value or a register
                if (value.StartsWith("%"))
                {
                    getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {value} to double");
                }
                else
                {
                    // It's a literal value, convert it directly
                    getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {value} to double");
                }
            }
            else if (currentType == "i8" || currentType == "i1")
            {
                string intReg = nextRegister();
                if (value.StartsWith("%"))
                {
                    getCurrentBody().AppendLine($"  {intReg} = zext {currentType} {value} to i32");
                }
                else
                {
                    getCurrentBody().AppendLine($"  {intReg} = zext {currentType} {value} to i32");
                }
                getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {intReg} to double");
            }
            else
            {
                getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {value} to double");
            }
            
            registerTypes[convReg] = "double";
            return convReg;
        }

        private string ConvertToInt(string value, string currentType)
        {
            if (currentType == "i32")
                return value;
            
            string convReg = nextRegister();
            if (currentType == "double")
            {
                getCurrentBody().AppendLine($"  {convReg} = fptosi double {value} to i32");
            }
            else if (currentType == "i8" || currentType == "i1")
            {
                getCurrentBody().AppendLine($"  {convReg} = zext {currentType} {value} to i32");
            }
            else
            {
                getCurrentBody().AppendLine($"  {convReg} = zext i32 {value} to i32");
            }
            
            registerTypes[convReg] = "i32";
            return convReg;
        }
    }
}