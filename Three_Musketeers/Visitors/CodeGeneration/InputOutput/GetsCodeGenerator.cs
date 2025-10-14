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
        private readonly StringBuilder declarations;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Func<string> nextRegister;
        private bool fgetsInitialized = false;
        private readonly VariableResolver variableResolver;

        public GetsCodeGenerator(
            StringBuilder declarations,
            Func<StringBuilder> getCurrentBody,
            Func<string> nextRegister,
            VariableResolver variableResolver)
        {
            this.declarations = declarations;
            this.getCurrentBody = getCurrentBody;
            this.nextRegister = nextRegister;
            this.variableResolver = variableResolver;
        }

        public string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            string varName = context.ID().GetText();

            Variable variable = variableResolver.GetVariable(varName)!;

            InitializeFgets();

            string bufferPtr = nextRegister();
            getCurrentBody().AppendLine($"  {bufferPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");

            string stdinReg = nextRegister();
            getCurrentBody().AppendLine($"  {stdinReg} = load %struct._IO_FILE*, %struct._IO_FILE** @stdin");

            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = call i8* @fgets(i8* {bufferPtr}, i32 256, %struct._IO_FILE* {stdinReg})");
            
            return null;
        }

        private void InitializeFgets()
        {
            if (fgetsInitialized)
                return;
                
            declarations.AppendLine("declare i8* @fgets(i8*, i32, %struct._IO_FILE*)");
            declarations.AppendLine("@stdin = external global %struct._IO_FILE*");
            declarations.AppendLine("%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }");
            declarations.AppendLine("%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }");
            
            fgetsInitialized = true;
        }

    }
}