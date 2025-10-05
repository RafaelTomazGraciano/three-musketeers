using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class GetsCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly StringBuilder mainBody;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<string> nextRegister;
        private bool fgetsInitialized = false;

        public GetsCodeGenerator(
            StringBuilder declarations,
            StringBuilder mainBody,
            Dictionary<string, Variable> variables,
            Func<string> nextRegister)
        {
            this.declarations = declarations;
            this.mainBody = mainBody;
            this.variables = variables;
            this.nextRegister = nextRegister;
        }

        public string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            string varName = context.ID().GetText();

            var variable = variables[varName];

            InitializeFgets();

            string bufferPtr = nextRegister();
            mainBody.AppendLine($"  {bufferPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");

            string stdinReg = nextRegister();
            mainBody.AppendLine($"  {stdinReg} = load %struct._IO_FILE*, %struct._IO_FILE** @stdin");

            string resultReg = nextRegister();
            mainBody.AppendLine($"  {resultReg} = call i8* @fgets(i8* {bufferPtr}, i32 256, %struct._IO_FILE* {stdinReg})");
            
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