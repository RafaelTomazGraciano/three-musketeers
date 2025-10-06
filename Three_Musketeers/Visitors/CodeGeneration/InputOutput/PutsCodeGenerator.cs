using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class PutsCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<string> nextRegister;
        private bool putsInitialized = false;

        public PutsCodeGenerator(
            StringBuilder declarations,
            Func<StringBuilder> getCurrentBody, 
            Dictionary<string, Variable> variables,
            Func<string> nextRegister)
        {
            this.declarations = declarations;
            this.getCurrentBody = getCurrentBody;
            this.variables = variables;
            this.nextRegister = nextRegister;
        }

        public string? VisitPutsStatement([NotNull] ExprParser.PutsStatementContext context)
{
    InitializePuts();

    if (context.ID() != null)
    {
        string varName = context.ID().GetText();

        var variable = variables[varName];

        string bufferPtr = nextRegister();
        getCurrentBody().AppendLine($"  {bufferPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");

        string resultReg = nextRegister();
        getCurrentBody().AppendLine($"  {resultReg} = call i32 @puts(i8* {bufferPtr})");
    }
    else if (context.STRING_LITERAL() != null)
    {
        string literal = context.STRING_LITERAL().GetText();
        literal = literal.Substring(1, literal.Length - 2).Replace("\\n", "\0A").Replace("\\t", "\09");

        string strLabel = $"@.str.puts.{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        int length = literal.Length + 1; 
        declarations.AppendLine($"{strLabel} = private unnamed_addr constant [{length} x i8] c\"{literal}\\00\"");

        string strPtr = nextRegister();
        getCurrentBody().AppendLine($"  {strPtr} = getelementptr inbounds [{length} x i8], [{length} x i8]* {strLabel}, i32 0, i32 0");

        string resultReg = nextRegister();
        getCurrentBody().AppendLine($"  {resultReg} = call i32 @puts(i8* {strPtr})");
    }

    return null;
}

        private void InitializePuts()
        {
            if (putsInitialized)
                return;
                
            declarations.AppendLine("declare i32 @puts(i8*)");
            
            putsInitialized = true;
        }
    }
} 