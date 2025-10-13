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
            
            // Handle type promotion
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
            
            // Handle multiplication and division (same as before)
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