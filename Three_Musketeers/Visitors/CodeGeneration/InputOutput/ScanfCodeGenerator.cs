using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class ScanfCodeGenerator
    {
        private readonly StringBuilder globalStrings;
        private readonly StringBuilder declarations;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, Variable> variables;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<string> nextStringLabel;
        private readonly Func<string, string> getLLVMType;
        private readonly VariableResolver variableResolver;
        private bool scanfInitialized = false;

        public ScanfCodeGenerator(
            StringBuilder globalStrings,
            StringBuilder declarations,
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, Variable> variables,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<string> nextStringLabel,
            Func<string, string> getLLVMType,
            VariableResolver variableResolver)
        {
            this.globalStrings = globalStrings;
            this.declarations = declarations;
            this.getCurrentBody = getCurrentBody;
            this.variables = variables;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.nextStringLabel = nextStringLabel;
            this.getLLVMType = getLLVMType;
            this.variableResolver = variableResolver;
        }

        public string? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            InitializeScanf(); 

            var ids = context.ID();
            if (ids.Length == 0)
            {
                return null;
            }

            var formatSpecifiers = new List<string>();
            var variablePointers = new List<string>();

            foreach (var idToken in ids)
            {
                string varName = idToken.GetText();

                Variable variable = variableResolver.GetVariable(varName);
                string llvmType = getLLVMType(variable.type);

                string formatSpec = GetFormatSpecifier(variable.type);
                formatSpecifiers.Add(formatSpec);

                variablePointers.Add($"{llvmType}* {variable.register}");
            }
            string formatStr = string.Join(" ", formatSpecifiers);
            string strLabel = nextStringLabel();
            int strLen = formatStr.Length + 1;

            globalStrings.AppendLine($"{strLabel} = private unnamed_addr constant [{strLen} x i8] c\"{formatStr}\\00\", align 1");

            string ptrReg = nextRegister();
            getCurrentBody().AppendLine($"  {ptrReg} = getelementptr inbounds [{strLen} x i8], [{strLen} x i8]* {strLabel}, i32 0, i32 0");

            var scanfArgs = new StringBuilder();
            scanfArgs.Append($"i8* {ptrReg}");

            for (int i = 0; i < ids.Length; i++) {
                string varName = ids[i].GetText();
                var variable = variables[varName];
                string llvmType = getLLVMType(variable.type);
                scanfArgs.Append($", {llvmType}* {variable.register}");
            }
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = call i32 (i8*, ...) @scanf({scanfArgs})");

            return null;
        }

        private string GetFormatSpecifier(string type)
        {
            return type switch
            {
                "int" => "%d",
                "double" => "%lf",
                "char" => "%c",
                "bool" => "%d",
                _ => "%d"
            };
        }

        private void InitializeScanf()
        {
            if (scanfInitialized)
                return;

            declarations.AppendLine("declare i32 @scanf(i8*, ...)");
            scanfInitialized = true;
        }
    }
}