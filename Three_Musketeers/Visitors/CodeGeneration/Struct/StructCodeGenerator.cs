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
        private readonly Func<ExprParser.IndexContext[], string> CalculateArrayPosition;
        public StructCodeGenerator(Dictionary<string, HeterogenousType> structsTypes, StringBuilder structDeclaration, Func<StringBuilder> getCurrentBody, Dictionary<string, string> registerTypes, Func<string> nextRegister, Dictionary<string, Variable> variables, Func<IParseTree, string> visit, Func<string, string> getLLVMType, Func<string, int> getSize, Func<ExprParser.IndexContext[], string> CalculateArrayPosition)
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
            this.CalculateArrayPosition = CalculateArrayPosition;
        }

        public string? VisitStructStatement(ExprParser.StructStatementContext context)
        {
            string structName = context.ID().GetText();
            string LLVMName = '%' + structName;
            var declarations = context.declaration();
            List<HeterogenousMember> variables = [];
            structDeclaration.AppendLine($"{LLVMName} = type {{");
            foreach (var declaration in declarations)
            {
                string decName = declaration.ID().GetText();
                string type = declaration.type().GetText();
                string llvmType = getLLVMType(type);
                if (type == "string") llvmType = "[256 x i8]";
                var indexes = declaration.intIndex();
                if (indexes.Length != 0)
                {
                    int len = indexes.Length;
                    string index = indexes[0].INT().GetText();
                    string result = $"{index} x ";
                    
                    foreach (var indexCtx in indexes.Skip(1))
                    {
                        index = indexCtx.GetText();
                        result += $"{index} x [";
                    }
                    llvmType = '[' + result + llvmType + new string(']', len);
                }
                structDeclaration.AppendLine($"   {llvmType},");
                variables.Add(new HeterogenousMember(decName, llvmType)); 
            }
            structsTypes[structName] = new StructType(LLVMName, variables, getSize);
            structDeclaration.Remove(structDeclaration.Length-2, 1);
            structDeclaration.AppendLine("}");
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
            var currentBody = getCurrentBody();
            string id = context.ID().GetText();
            var structContinueCtx = context.structContinue();
            var indexes = context.index();

            // Verificar se a variável existe
            if (!variables.ContainsKey(id))
            {
                throw new Exception($"Variable '{id}' not found");
            }

            Variable variable = variables[id];
            string currentRegister = variable.register;
            string currentType = variable.type;

            // Se houver índices de array na variável base (ex: structArray[0].member)
            if (indexes.Length > 0)
            {
                string arrayPositions = CalculateArrayPosition(indexes);
                string ptrReg = nextRegister();
                currentBody.AppendLine($"   {ptrReg} = getelementptr inbounds {currentType}, {currentType}* {currentRegister}, i32 0,{arrayPositions}");
                currentRegister = ptrReg;
                registerTypes[ptrReg] = currentType;
            }

            // Determinar o tipo de acesso (. ou ->)
            bool isPointerAccess = context.GetText().Contains("->");

            // Processar o acesso ao membro
            return ProcessStructContinue(structContinueCtx, currentRegister, currentType, isPointerAccess);
        }

        private string ProcessStructContinue(ExprParser.StructContinueContext context, string currentRegister, string currentType, bool isPointerAccess)
        {
            var currentBody = getCurrentBody();

            // Se for acesso por ponteiro, fazer load primeiro
            if (isPointerAccess)
            {
                string loadReg = nextRegister();
                currentBody.AppendLine($"   {loadReg} = load {currentType}, {currentType}* {currentRegister}");
                currentRegister = loadReg;
                registerTypes[loadReg] = currentType;
            }

            // Remover o '%' do nome do tipo para buscar na tabela de structs
            string structName = currentType.TrimStart('%');

            // Obter informações da struct
            if (!structsTypes.ContainsKey(structName))
            {
                throw new Exception($"Struct type '{structName}' not found");
            }

            StructType structType = (StructType)structsTypes[structName];

            // Usar o LLVMName diretamente da StructType (já contém o '%')
            string llvmStructType = structType.GetLLVMName();

            // Se structContinue é outro structGet (acesso aninhado)
            if (context.structGet() != null)
            {
                string memberId = context.structGet().ID().GetText();
                var indexes = context.structGet().index();

                // Obter informações do membro usando os métodos da StructType
                int memberIndex = structType.GetFieldIndex(memberId);
                string memberType = structType.GetFieldType(memberId);

                // Gerar getelementptr para acessar o membro
                string memberPtrReg = nextRegister();
                currentBody.AppendLine($"   {memberPtrReg} = getelementptr inbounds {llvmStructType}, {llvmStructType}* {currentRegister}, i32 0, i32 {memberIndex}");
                registerTypes[memberPtrReg] = memberType;

                // Processar índices de array no membro (se houver)
                if (indexes.Length > 0)
                {
                    string arrayPositions = CalculateArrayPosition(indexes);
                    string arrayPtrReg = nextRegister();
                    currentBody.AppendLine($"   {arrayPtrReg} = getelementptr inbounds {memberType}, {memberType}* {memberPtrReg}, i32 0,{arrayPositions}");
                    memberPtrReg = arrayPtrReg;
                    registerTypes[arrayPtrReg] = memberType;
                }

                // Continuar processando recursivamente
                bool nextIsPointerAccess = context.structGet().GetText().Contains("->");
                return ProcessStructContinue(context.structGet().structContinue(), memberPtrReg, memberType, nextIsPointerAccess);
            }
            else
            {
                // Caso base: acessar o membro final
                string memberId = context.ID().GetText();
                var indexes = context.index();

                // Obter informações do membro usando os métodos da StructType
                int memberIndex = structType.GetFieldIndex(memberId);
                string memberType = structType.GetFieldType(memberId);

                // Gerar getelementptr para o membro
                string memberPtrReg = nextRegister();
                currentBody.AppendLine($"   {memberPtrReg} = getelementptr inbounds {llvmStructType}, {llvmStructType}* {currentRegister}, i32 0, i32 {memberIndex}");
                registerTypes[memberPtrReg] = memberType;

                // Processar índices de array (se houver)
                if (indexes.Length > 0)
                {
                    string arrayPositions = CalculateArrayPosition(indexes);
                    string arrayPtrReg = nextRegister();
                    currentBody.AppendLine($"   {arrayPtrReg} = getelementptr inbounds {memberType}, {memberType}* {memberPtrReg}, i32 0,{arrayPositions}");
                    memberPtrReg = arrayPtrReg;
                    registerTypes[arrayPtrReg] = memberType;
                }

                return memberPtrReg;
            }
        }

        public string VisitVarStruct(ExprParser.StructGetContext structGetContext)
        {
            string register = VisitStructGet(structGetContext);
            string registerType = registerTypes[register];
            var currentBody = getCurrentBody();
            string loadReg = nextRegister();
            currentBody.AppendLine($"   {loadReg} = load {registerType}, {registerType}* {register}");
            registerTypes[loadReg] = registerType;
            return loadReg;
        }
    }
}