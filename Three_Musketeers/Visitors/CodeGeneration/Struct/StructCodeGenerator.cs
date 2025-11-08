using System.Collections;
using System.Text;
using Antlr4.Runtime.Tree;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.Struct
{
    public class StructCodeGenerator
    {
        private readonly Dictionary<string, HeterogenousType> structsTypes;
        private readonly StringBuilder structDeclaration;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly VariableResolver variableResolver;
        private readonly Func<IParseTree, string> visit;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<string, int> getSize;
        private readonly Func<ExprParser.IndexContext[], string> CalculateArrayPosition;
        
        public StructCodeGenerator(Dictionary<string, HeterogenousType> structsTypes, StringBuilder structDeclaration, Func<StringBuilder> getCurrentBody, Dictionary<string, string> registerTypes, Func<string> nextRegister, VariableResolver variableResolver, Func<IParseTree, string> visit, Func<string, string> getLLVMType, Func<string, int> getSize, Func<ExprParser.IndexContext[], string> CalculateArrayPosition)
        {
            this.structsTypes = structsTypes;
            this.structDeclaration = structDeclaration;
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.variableResolver = variableResolver;
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
                string llvmType = ProcessDeclaration(declaration);
                structDeclaration.AppendLine($"   {llvmType},");
                variables.Add(new HeterogenousMember(decName, llvmType));
            }
            structsTypes[structName] = new StructType(LLVMName, variables, getSize);
            structDeclaration.Remove(structDeclaration.Length - 2, 1);
            structDeclaration.AppendLine("}");
            return null;
        }

        public string? VisitUnionStatement(ExprParser.UnionStatementContext context)
        {
            string unionName = context.ID().GetText();
            string LLVMName = '%' + unionName;
            var declarations = context.declaration();
            List<HeterogenousMember> members = [];
            
            foreach (var declaration in declarations)
            {
                string decName = declaration.ID().GetText();
                string llvmType = ProcessDeclaration(declaration);
                members.Add(new HeterogenousMember(decName, llvmType));
            }
            
            // Create the UnionType - it will calculate the largest type automatically
            var unionType = new UnionType(LLVMName, members, getSize);
            structsTypes[unionName] = unionType;
            
            // Declare the union as a struct with a single field of the largest type
            string largestType = unionType.GetLargestMemberType();
            structDeclaration.AppendLine($"{LLVMName} = type {{");
            structDeclaration.AppendLine($"   {largestType}");
            structDeclaration.AppendLine("}");
            
            return null;
        }
        
        private string ProcessDeclaration(ExprParser.DeclarationContext context)
        {
            string type = context.type().GetText();
            string llvmType = getLLVMType(type);
            if (type == "string") llvmType = "[256 x i8]";
            var indexes = context.intIndex();
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
            return llvmType;
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
            Variable variable = variableResolver.GetVariable(id)!;
            string currentRegister = variable.register;
            string currentType = variable.type;

            if (indexes.Length > 0)
            {
                string arrayPositions = CalculateArrayPosition(indexes);
                string ptrReg = nextRegister();
                
                string varLLVMType = variable.LLVMType;
                currentBody.AppendLine($"   {ptrReg} = getelementptr inbounds {varLLVMType}, {varLLVMType}* {currentRegister}, i32 0,{arrayPositions}");
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

            string typeName = currentType.TrimStart('%');

            if (!structsTypes.ContainsKey(typeName))
            {
                throw new Exception($"Type '{typeName}' not found");
            }

            HeterogenousType heterogeneousType = structsTypes[typeName];

            if (context.structGet() != null)
            {
                string memberId = context.structGet().ID().GetText();
                var indexes = context.structGet().index();

                string memberPtrReg = "";
                string memberType = "";

                if (heterogeneousType is StructType structType)
                {
                    string llvmStructType = structType.GetLLVMName();
                
                    if (isPointerAccess)
                    {
                        string loadReg = nextRegister();
                        currentBody.AppendLine($"   {loadReg} = load {llvmStructType}, {llvmStructType}* {currentRegister}");
                        currentRegister = loadReg;
                        registerTypes[loadReg] = llvmStructType;
                    }

                    int memberIndex = structType.GetFieldIndex(memberId);
                    memberType = structType.GetFieldType(memberId);

                    memberPtrReg = nextRegister();
                    currentBody.AppendLine($"   {memberPtrReg} = getelementptr inbounds {llvmStructType}, {llvmStructType}* {currentRegister}, i32 0, i32 {memberIndex}");
                }
                else if (heterogeneousType is UnionType unionType)
                {
                    memberType = GetMemberType(heterogeneousType, memberId);

                    memberPtrReg = nextRegister();
                    string bitcastExpr = unionType.GetLLVMVar(memberId, currentRegister);
                    currentBody.AppendLine($"   {memberPtrReg} = {bitcastExpr}");
                }
                

                registerTypes[memberPtrReg] = memberType;

                if (indexes.Length > 0)
                {
                    string arrayPositions = CalculateArrayPosition(indexes);
                    string arrayPtrReg = nextRegister();
                    currentBody.AppendLine($"   {arrayPtrReg} = getelementptr inbounds {memberType}, {memberType}* {memberPtrReg}, i32 0,{arrayPositions}");
                    memberPtrReg = arrayPtrReg;
                    registerTypes[arrayPtrReg] = GetInnerType(memberType);
                }

                bool nextIsPointerAccess = context.structGet().GetText().Contains("->");
                string nextType = GetBaseType(memberType);
                
                return ProcessStructContinue(context.structGet().structContinue(), memberPtrReg, nextType, nextIsPointerAccess);
            }
            else
            {
                string memberId = context.ID().GetText();
                var indexes = context.index();

                string memberPtrReg;
                string memberType;

                if (heterogeneousType is StructType structType)
                {
                    string llvmStructType = structType.GetLLVMName();
                    
                    if (isPointerAccess)
                    {
                        string loadReg = nextRegister();
                        currentBody.AppendLine($"   {loadReg} = load {llvmStructType}, {llvmStructType}* {currentRegister}");
                        currentRegister = loadReg;
                        registerTypes[loadReg] = llvmStructType;
                    }

                    // Para structs, usar getelementptr com índice
                    int memberIndex = structType.GetFieldIndex(memberId);
                    memberType = structType.GetFieldType(memberId);

                    memberPtrReg = nextRegister();
                    currentBody.AppendLine($"   {memberPtrReg} = getelementptr inbounds {llvmStructType}, {llvmStructType}* {currentRegister}, i32 0, i32 {memberIndex}");
                }
                else if (heterogeneousType is UnionType unionType)
                {
                    // Para unions, fazer bitcast direto
                    memberType = GetMemberType(heterogeneousType, memberId);

                    memberPtrReg = nextRegister();
                    string bitcastExpr = unionType.GetLLVMVar(memberId, currentRegister);
                    currentBody.AppendLine($"   {memberPtrReg} = {bitcastExpr}");
                }
                else
                {
                    throw new Exception($"Unknown heterogeneous type");
                }

                registerTypes[memberPtrReg] = memberType;

                // Processar índices de array (se houver)
                if (indexes.Length > 0)
                {
                    string arrayPositions = CalculateArrayPosition(indexes);
                    string arrayPtrReg = nextRegister();
                    currentBody.AppendLine($"   {arrayPtrReg} = getelementptr inbounds {memberType}, {memberType}* {memberPtrReg}, i32 0,{arrayPositions}");
                    memberPtrReg = arrayPtrReg;
                    registerTypes[arrayPtrReg] = GetInnerType(memberType);
                }

                return memberPtrReg;
            }
        }

        // Método auxiliar para obter o tipo de um membro
        private string GetMemberType(HeterogenousType heterogeneousType, string memberName)
        {
            var members = heterogeneousType.GetMembers();
            var member = members.FirstOrDefault(m => m.name == memberName);

            if (member == null)
            {
                throw new Exception($"Member '{memberName}' not found in type '{heterogeneousType.GetLLVMName()}'");
            }

            return member.LLVMType;
        }

        private string GetInnerType(string arrayType)
        {
            if (arrayType.StartsWith("["))
            {
                int lastX = arrayType.LastIndexOf(" x ");
                if (lastX != -1)
                {
                    return arrayType.Substring(lastX + 3).TrimEnd(']');
                }
            }
            return arrayType;
        }

        private string GetBaseType(string type)
        {
            if (type.StartsWith("%"))
            {
                return type;
            }

            string innerType = GetInnerType(type);
            if (innerType != type && innerType.StartsWith("%"))
            {
                return innerType;
            }

            return type;
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