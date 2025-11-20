using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class GetsCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Func<string> nextRegister;
        private readonly VariableResolver variableResolver;
        private readonly Func<ExprParser.StructGetContext, string> visitStructGet;
        private readonly Dictionary<string, string> registerTypes;

        public GetsCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Func<string> nextRegister,
            VariableResolver variableResolver,
            Func<ExprParser.StructGetContext, string> visitStructGet,
            Dictionary<string, string> registerTypes)
        {
            this.getCurrentBody = getCurrentBody;
            this.nextRegister = nextRegister;
            this.variableResolver = variableResolver;
            this.visitStructGet = visitStructGet;
            this.registerTypes = registerTypes;
        }

        public string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            var currentBody = getCurrentBody();
            string bufferPtr;

            // Struct/Union member access
            if (context.structGet() != null)
            {
                var structGetCtx = context.structGet();
                
                // VisitStructGet returns a pointer to the member
                string memberPtrReg = visitStructGet(structGetCtx);
                
                if (memberPtrReg == null || !registerTypes.ContainsKey(memberPtrReg))
                {
                    return null;
                }

                // The member should be [256 x i8]
                bufferPtr = nextRegister();
                currentBody.AppendLine($"  {bufferPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {memberPtrReg}, i32 0, i32 0");
            }
            else
            {
                // Simple variable
                string varName = context.ID().GetText();
                Variable variable = variableResolver.GetVariable(varName)!;

                bufferPtr = nextRegister();
                currentBody.AppendLine($"  {bufferPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");
            }

            string stdinReg = nextRegister();
            currentBody.AppendLine($"  {stdinReg} = load %struct._IO_FILE*, %struct._IO_FILE** @stdin");

            string resultReg = nextRegister();
            currentBody.AppendLine($"  {resultReg} = call i8* @fgets(i8* {bufferPtr}, i32 256, %struct._IO_FILE* {stdinReg})");
            
            return null;
        }
    }
}