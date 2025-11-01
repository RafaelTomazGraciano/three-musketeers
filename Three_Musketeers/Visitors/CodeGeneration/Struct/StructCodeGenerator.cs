using System.Collections;
using System.Text;
using Antlr4.Runtime.Tree;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Struct
{
    public class StructCodeGenerator
    {
        private readonly Dictionary<string, HeterogenousType> structsTypes;
        private readonly StringBuilder structDeclaration;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<IParseTree, string> visit;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<string, int> getSize;
        public StructCodeGenerator(Dictionary<string, HeterogenousType> structsTypes, StringBuilder structDeclaration, Func<StringBuilder> getCurrentBody, Dictionary<string, string> registerTypes, Func<string> nextRegister, Dictionary<string, Variable> variables, Func<IParseTree, string> visit, Func<string, string> getLLVMType, Func<string, int> getSize)
        {
            this.structsTypes = structsTypes;
            this.structDeclaration = structDeclaration;
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.variables = variables;
            this.visit = visit;
            this.getLLVMType = getLLVMType;
            this.getSize = getSize;
        }

        public string? VisitStructStatement(ExprParser.StructStatementContext context)
        {
            string structName = context.ID().GetText();
            string LLVMName = '%' + structName;
            var declarations = context.declaration();
            foreach (var declaration in declarations)
            {
                if (declaration != null)
                {
                    var indexes = declaration.intIndex();
                }
            }
            return null;
        }

        public string? VisitStructAtt(ExprParser.StructAttContext context)
        {
            var currentBody = getCurrentBody();
            string register = VisitStructGet(context.structGet());
            string registerType = registerTypes[register];
            string expr = visit(context.expr());
            string exprType = registerTypes[expr];

            currentBody.AppendLine($"   store {exprType} {expr}, {registerType}* {register}");
            return null;
        }

        public string VisitStructGet(ExprParser.StructGetContext context)
        {
            return "";
        }

        public string VisitVarStruct(ExprParser.VarStructContext context)
        {
            return "";
        }
    }
}