using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Functions
{
    public class FunctionCallCodeGenerator
    {
        private readonly StringBuilder codeOutput;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Dictionary<string, FunctionInfo> declaredFunctions;
        private readonly Func<string> nextRegister;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly Func<StringBuilder> getCurrentBody;

        public FunctionCallCodeGenerator(
            Dictionary<string, string> registerTypes,
            Dictionary<string, FunctionInfo> declaredFunctions,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.ExprContext, string?> visitExpression,
            Func<StringBuilder> getCurrentBody)
        {
            this.codeOutput = new StringBuilder();
            this.registerTypes = registerTypes;
            this.declaredFunctions = declaredFunctions;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitExpression = visitExpression;
            this.getCurrentBody = getCurrentBody;
        }

        public string? VisitFunctionCall([NotNull] ExprParser.FunctionCallContext context)
        {
            string functionName = context.ID().GetText();

            FunctionInfo functionInfo = declaredFunctions[functionName];

            // Get return type in LLVM format
            string llvmReturnType;
            if (functionInfo.isVoid)
            {
                llvmReturnType = "void";
            }
            else
            {
                llvmReturnType = getLLVMType(functionInfo.returnType!);

                // handle array return types
                if (functionInfo.isArray && functionInfo.returnDimensions != null)
                {
                    for (int i = 0; i < functionInfo.returnDimensions.Count; i++)
                    {
                        llvmReturnType += "*";
                    }
                }
            }
            
            // evaluate arguments
            List<string> argRegisters = new List<string>();
            List<string> argTypes = new List<string>();
            
            var providedArgs = context.expr();
            if (providedArgs != null)
            {
                for (int i = 0; i < providedArgs.Length; i++)
                {
                    string? argReg = visitExpression(providedArgs[i]);
                    if (argReg == null)
                    {
                        return null;
                    }

                    string argType = registerTypes.ContainsKey(argReg) 
                        ? registerTypes[argReg] 
                        : "i32";

                    argRegisters.Add(argReg);
                    argTypes.Add(argType);
                }
            }

            // build argument list for call instruction
            List<string> callArgs = new List<string>();
            for (int i = 0; i < argRegisters.Count; i++)
            {
                callArgs.Add($"{argTypes[i]} {argRegisters[i]}");
            }
            string argList = string.Join(", ", callArgs);

            // generate call instruction
            StringBuilder body = GetCurrentBody();

            if (functionInfo.isVoid)
            {
                // void function call
                body.AppendLine($"  call void @{functionName}({argList})");
                return null; // void functions don't return a value
            }
            else
            {
                // non-void function call
                string resultReg = nextRegister();
                body.AppendLine($"  {resultReg} = call {llvmReturnType} @{functionName}({argList})");
                registerTypes[resultReg] = llvmReturnType;
                return resultReg;
            }
        }

        private StringBuilder GetCurrentBody()
        {
            return getCurrentBody();
        }
    }
}