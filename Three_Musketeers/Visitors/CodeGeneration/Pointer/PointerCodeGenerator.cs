using System.Diagnostics.CodeAnalysis;
using System.Text;
using LLVMSharp;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.Pointer
{

    public class PointerCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;
        private readonly VariableResolver variableResolver;
        private readonly Func<ExprParser.IndexContext[], string> CalculateArrayPosition;

        public PointerCodeGenerator(
        Func<StringBuilder> getCurrentBody,
        Dictionary<string, string> registerTypes,
        Func<string> nextRegister,
        Func<ExprParser.ExprContext, string> visitExpression,
        VariableResolver variableResolver,
        Func<ExprParser.IndexContext[], string> CalculateArrayPosition) 
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.variableResolver = variableResolver;
            this.CalculateArrayPosition = CalculateArrayPosition;
        }

        public string? VisitExprAddress([NotNull] ExprParser.ExprAddressContext context)
        {
            var expr = context.expr();
            Variable variable;

            if (expr is ExprParser.VarContext varContext)
            {
                string varName = varContext.ID().GetText();
                variable = variableResolver.GetVariable(varName)!;

                if (variable != null)
                {
                    // Return the address (register) of the variable
                    registerTypes[variable.register] = variable.LLVMType + "*";
                    return variable.register;
                }
            }
            else if (expr is ExprParser.VarArrayContext arrayContext)
            {
                // For array elements, we need the pointer, not the value
                string varName = arrayContext.ID().GetText();
                var indexes = arrayContext.index();

                // Get pointer to element (don't load the value)
                variable = variableResolver.GetVariable(varName)!;
                if (variable == null) return null;

                var currentBody = getCurrentBody();
                var llvmType = variable.LLVMType;
                bool isPointer = !(variable is ArrayVariable) && llvmType.Contains('*');

                if (isPointer)
                {
                    string loadedPointer;

                    loadedPointer = nextRegister();
                    currentBody.AppendLine($"  {loadedPointer} = load {llvmType}, {llvmType}* {variable.register}, align 8");   

                    // Get element address with single index (no i32 0)
                    string indexExpr = visitExpression(indexes[0].expr());
                    string gepReg = nextRegister();
                    string elementType = CodeGeneratorBase.RemoveOneAsterisk(llvmType);
                    currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {elementType}, {llvmType} {loadedPointer}, i32 {indexExpr}");

                    registerTypes[gepReg] = elementType + "*";
                    return gepReg;
                }
                else if (variable is ArrayVariable arrayVar)
                {
                    string positions = CalculateArrayPosition(indexes);
                    string gepReg = nextRegister();
                    currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {llvmType}, {llvmType}* {variable.register}, i32 0, {positions}");

                    registerTypes[gepReg] = arrayVar.innerType + "*";
                    return gepReg;
                }
            }

            return null;
        }


        public string VisitDerref(ExprParser.DerrefContext context)
        {
            string reg = visitExpression(context.expr());

            string pointerType = registerTypes[reg];
            string baseType = pointerType.Substring(0, pointerType.Length - 1);
            string result = nextRegister();

            getCurrentBody().AppendLine($"  {result} = load {baseType}, {pointerType} {reg}");
            registerTypes[result] = baseType;
            return result;
        }

        public string? VisitDerrefAtt(ExprParser.DerrefAttContext context)
        {
            string pointerReg = visitExpression(context.derref().expr());
            string expr = visitExpression(context.expr());
            string pointerType = registerTypes[pointerReg];

            string baseType = pointerType.EndsWith('*')
                ? pointerType.Substring(0, pointerType.Length - 1)
                : pointerType;

            getCurrentBody().AppendLine($"  store {baseType} {expr}, {pointerType} {pointerReg}");

            return null;
        }
    }
}