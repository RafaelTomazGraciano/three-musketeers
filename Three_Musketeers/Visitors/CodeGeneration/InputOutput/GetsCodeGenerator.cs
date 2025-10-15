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

        public GetsCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Func<string> nextRegister,
            VariableResolver variableResolver)
        {
            this.getCurrentBody = getCurrentBody;
            this.nextRegister = nextRegister;
            this.variableResolver = variableResolver;
        }

        public string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            string varName = context.ID().GetText();

            Variable variable = variableResolver.GetVariable(varName)!;

            string bufferPtr = nextRegister();
            getCurrentBody().AppendLine($"  {bufferPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");

            string stdinReg = nextRegister();
            getCurrentBody().AppendLine($"  {stdinReg} = load %struct._IO_FILE*, %struct._IO_FILE** @stdin");

            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = call i8* @fgets(i8* {bufferPtr}, i32 256, %struct._IO_FILE* {stdinReg})");
            
            return null;
        }

    }
}