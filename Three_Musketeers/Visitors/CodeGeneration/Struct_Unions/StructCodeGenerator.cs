using System.Collections;
using System.Text;
using Antlr4.Runtime.Tree;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.Struct_Unions
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
        
        public StructCodeGenerator(
            Dictionary<string, HeterogenousType> structsTypes, 
            StringBuilder structDeclaration, 
            Func<StringBuilder> getCurrentBody, 
            Dictionary<string, string> registerTypes, 
            Func<string> nextRegister, 
            VariableResolver variableResolver, 
            Func<IParseTree, string> visit, 
            Func<string, string> getLLVMType, 
            Func<string, int> getSize, 
            Func<ExprParser.IndexContext[], string> CalculateArrayPosition)
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
                var declType = declaration.type();
                
                // Get base LLVM type
                string llvmType;
                
                // Handle "struct TypeName" syntax
                if (declType.ChildCount >= 2 && 
                    (declType.GetChild(0).GetText() == "struct" || declType.GetChild(0).GetText() == "union"))
                {
                    string keyword = declType.GetChild(0).GetText();
                    string referencedName = declType.ID().GetText();
                    
                    if (keyword == "struct")
                    {
                        if (referencedName == structName)
                        {
                            // Self-referential struct
                            llvmType = LLVMName;
                        }
                        else if (structsTypes.ContainsKey(referencedName))
                        {
                            llvmType = structsTypes[referencedName].GetLLVMName();
                        }
                        else
                        {
                            llvmType = '%' + referencedName;
                        }
                    }
                    else // union
                    {
                        if (structsTypes.ContainsKey(referencedName))
                        {
                            llvmType = structsTypes[referencedName].GetLLVMName();
                        }
                        else
                        {
                            llvmType = '%' + referencedName;
                        }
                    }
                }
                else if (structsTypes.ContainsKey(declType.GetText()))
                {
                    llvmType = structsTypes[declType.GetText()].GetLLVMName();
                }
                else
                {
                    // Primitive type
                    string primitiveType = declType.GetText();
                    llvmType = getLLVMType(primitiveType);
                    if (primitiveType == "string") llvmType = "[256 x i8]";
                }
                
                // Process pointers
                var pointerTokens = declaration.POINTER();
                int pointerLevel = pointerTokens?.Length ?? 0;
                
                for (int i = 0; i < pointerLevel; i++)
                {
                    llvmType += "*";
                }
                
                // Process arrays
                var indexes = declaration.intIndex();
                if (indexes.Length != 0)
                {
                    // Build nested array type from innermost to outermost
                    for (int i = indexes.Length - 1; i >= 0; i--)
                    {
                        string size = indexes[i].INT().GetText();
                        llvmType = $"[{size} x {llvmType}]";
                    }
                }
                
                structDeclaration.AppendLine($"   {llvmType},");
                variables.Add(new HeterogenousMember(decName, llvmType));
            }
            
            structsTypes[structName] = new StructType(LLVMName, variables, getSize);
            
            // Remove trailing comma
            structDeclaration.Remove(structDeclaration.Length - 2, 1);
            structDeclaration.AppendLine("}");
            
            return null;
        }

        public string? VisitStructAtt(ExprParser.StructAttContext context)
        {
            var currentBody = getCurrentBody();
            string register = VisitStructGet(context.structGet());
            
            if (!registerTypes.ContainsKey(register))
            {
                throw new Exception($"Register type not found for {register}");
            }
            
            string fieldPtrType = registerTypes[register];
            string fieldType = fieldPtrType.TrimEnd('*');
            
            string expr = visit(context.expr());
            
            if (fieldType.Contains('*') && expr == "0")
            {
                expr = "null";
            }
            
            string valueType = registerTypes.ContainsKey(expr) ? registerTypes[expr] : fieldType;

            if (fieldType == "i8" && valueType == "i32")
            {
                string truncReg = nextRegister();
                currentBody.AppendLine($"   {truncReg} = trunc i32 {expr} to i8");
                registerTypes[truncReg] = "i8";
                expr = truncReg;
                valueType = "i8";
            }

            if (fieldType.StartsWith("%") && fieldType.EndsWith("*") && 
                valueType.StartsWith("%") && valueType.EndsWith("*") && 
                fieldType != valueType)
            {
                string bitcastReg = nextRegister();
                currentBody.AppendLine($"   {bitcastReg} = bitcast {valueType} {expr} to {fieldType}");
                expr = bitcastReg;
                valueType = fieldType;
            }

            if (fieldType == "[256 x i8]" && valueType == "i8*")
            {
                string destPtr = nextRegister();
                currentBody.AppendLine($"   {destPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {register}, i32 0, i32 0");
                string strcpyResult = nextRegister();
                currentBody.AppendLine($"   {strcpyResult} = call i8* @strcpy(i8* {destPtr}, i8* {expr})");
            }
            else
            {
                // Determine alignment based on the UNION type
                int alignment;
                if (fieldPtrType.StartsWith("%"))
                {
                    // It's a union/struct - use natural alignment of value being stored
                    alignment = valueType == "double" ? 8 : 
                                valueType.Contains("*") ? 8 :
                                valueType == "i32" ? 4 : 
                                valueType == "i8" ? 1 : 4;
                }
                else
                {
                    alignment = valueType == "double" ? 8 : 
                                valueType == "i32" ? 4 : 
                                valueType == "i8" ? 1 : 4;
                }
                
                currentBody.AppendLine($"   store {valueType} {expr}, {fieldPtrType} {register}, align {alignment}");
            }
            
            return null;
        }

        public string VisitStructGet(ExprParser.StructGetContext context)
        {
            var currentBody = getCurrentBody();
            var structContinueCtx = context.structContinue();
            var indexes = context.index();
            
            string currentRegister;
            string currentType;
            
            // Check if this is a dereferenced pointer access: (*ptr).member
            if (context.GetChild(0).GetText() == "(")
            {
                var pointerToken = context.POINTER();
                if (pointerToken == null)
                {
                    throw new Exception("Invalid dereference syntax in struct access");
                }
                
                var exprContext = context.expr();
                if (exprContext == null)
                {
                    throw new Exception("Missing expression in dereference");
                }
                
                // Get the variable being dereferenced
                if (exprContext is ExprParser.VarContext varCtx)
                {
                    string varName = varCtx.ID().GetText();
                    Variable? variable = variableResolver.GetVariable(varName);
                    if (variable == null)
                    {
                        throw new Exception($"Variable '{varName}' not found");
                    }
                    
                    string pointerReg = variable.register;
                    string pointerType = variable.LLVMType;  

                    // Remove one * to get the base struct type
                    string baseStructType = CodeGeneratorBase.RemoveOneAsterisk(pointerType);  
                    
                    if (variable.isDirectPointerParam)
                    {
                        currentRegister = pointerReg;
                        currentType = baseStructType;
                        registerTypes[pointerReg] = pointerType;
                    }
                    else
                    {
                        string loadedPtrReg = nextRegister();
                        currentBody.AppendLine($"   {loadedPtrReg} = load {pointerType}, {pointerType}* {pointerReg}");
                        
                        currentRegister = loadedPtrReg;
                        currentType = baseStructType; 
                        registerTypes[loadedPtrReg] = pointerType;
                    }
                }
                else if (exprContext is ExprParser.VarStructContext varStructCtx)
                {
                    string innerStructReg = VisitStructGet(varStructCtx.structGet());
                    string innerStructType = registerTypes[innerStructReg];
                    
                    string innerBaseType = CodeGeneratorBase.RemoveOneAsterisk(innerStructType);
                    
                    string loadedPtrReg = nextRegister();
                    currentBody.AppendLine($"   {loadedPtrReg} = load {innerBaseType}, {innerStructType} {innerStructReg}");
                    
                    currentRegister = loadedPtrReg;
                    currentType = CodeGeneratorBase.RemoveOneAsterisk(innerBaseType);
                    registerTypes[loadedPtrReg] = innerBaseType;
                }
                else if (exprContext is ExprParser.DerrefExprContext derrefExprCtx)
                {
                    string innerReg = visit(derrefExprCtx);
                    string innerType = registerTypes[innerReg];
                    
                    currentRegister = innerReg;
                    currentType = CodeGeneratorBase.RemoveOneAsterisk(innerType);
                }
                else
                {
                    throw new Exception("Complex expressions in dereference not yet supported in code generation");
                }
            }
            else
            {
                // Regular struct access (ID.member or ID->member)
                string id = context.ID().GetText();
                Variable? variable = variableResolver.GetVariable(id);
                
                if (variable == null)
                {
                    throw new Exception($"Variable '{id}' not found");
                }
                
                string pointerReg = variable.register;
                string pointerType = variable.LLVMType; 

                // Check if this is a pointer type or array of structs
                bool isPointerType = pointerType.EndsWith("*");
                bool isArrayOfStructs = pointerType.StartsWith("[") && pointerType.Contains("%");
                
                if (isPointerType)
                {
                    string baseType = CodeGeneratorBase.RemoveOneAsterisk(pointerType);
                    
                    if (variable.isDirectPointerParam)
                    {
                        currentRegister = pointerReg;
                        currentType = baseType;
                    }
                    else
                    {
                        string loadedPtrReg = nextRegister();
                        currentBody.AppendLine($"   {loadedPtrReg} = load {pointerType}, {pointerType}* {pointerReg}");
                        
                        currentRegister = loadedPtrReg;
                        currentType = baseType;
                        registerTypes[loadedPtrReg] = pointerType;
                    }
                }
                else if (isArrayOfStructs)
                {
                    currentRegister = pointerReg;
                    
                    // Extract struct type from array type: [3 x %Student] -> %Student
                    int xIndex = pointerType.LastIndexOf(" x ");
                    if (xIndex != -1)
                    {
                        currentType = pointerType.Substring(xIndex + 3).TrimEnd(']');
                    }
                    else
                    {
                        currentType = pointerType;
                    }
                }
                else
                {
                    currentRegister = pointerReg;
                    currentType = pointerType; 
                }

                if (indexes.Length > 0)
                {
                    if (isPointerType)
                    {
                        string indexExpr = visit(indexes[0].expr());
                        string gepReg = nextRegister();
                        
                        currentBody.AppendLine($"   {gepReg} = getelementptr inbounds {currentType}, {currentType}* {currentRegister}, i32 {indexExpr}");
                        
                        currentRegister = gepReg;
                        registerTypes[gepReg] = currentType + "*";
                    }
                    else
                    {
                        string arrayPositions = CalculateArrayPosition(indexes);
                        string ptrReg = nextRegister();
                        
                        string varLLVMType = variable.LLVMType;
                        currentBody.AppendLine($"   {ptrReg} = getelementptr inbounds {varLLVMType}, {varLLVMType}* {currentRegister}, i32 0, {arrayPositions}");
                        
                        currentRegister = ptrReg;
                        registerTypes[ptrReg] = currentType + "*";
                    }
                }
            }

            return ProcessStructContinue(structContinueCtx, currentRegister, currentType, false);
        }

        private string ProcessStructContinue(ExprParser.StructContinueContext context, string currentRegister, string currentType, bool isPointerAccess)
        {
            var currentBody = getCurrentBody();

            if (!currentType.StartsWith("%") && !currentType.StartsWith("struct") && !currentType.StartsWith("union"))
            {
                if (!registerTypes.ContainsKey(currentRegister))
                {
                    registerTypes[currentRegister] = currentType + "*";
                }
                return currentRegister;
            }

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
                    
                    bool isAlreadyPointer = registerTypes.ContainsKey(currentRegister) && registerTypes[currentRegister].EndsWith("*");
                    
                    if (isPointerAccess && !isAlreadyPointer)
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

                    registerTypes[memberPtrReg] = memberType + "*";
                }
                else if (heterogeneousType is UnionType unionType)
                {
                    memberType = GetMemberType(heterogeneousType, memberId);

                    memberPtrReg = nextRegister();
                    
                    if (memberType.StartsWith("%"))
                    {
                        // Member is a struct/union - bitcast to that type
                        currentBody.AppendLine($"   {memberPtrReg} = bitcast {unionType.GetLLVMName()}* {currentRegister} to {memberType}*");
                        registerTypes[memberPtrReg] = memberType + "*";
                    }
                    else
                    {
                        // Member is a primitive type - bitcast directly
                        string bitcastExpr = unionType.GetLLVMVar(memberId, currentRegister);
                        currentBody.AppendLine($"   {memberPtrReg} = {bitcastExpr}");
                        registerTypes[memberPtrReg] = memberType + "*";
                    }
                }

                if (indexes.Length > 0)
                {
                    string arrayPositions = CalculateArrayPosition(indexes);
                    string arrayPtrReg = nextRegister();
                    currentBody.AppendLine($"   {arrayPtrReg} = getelementptr inbounds {memberType}, {memberType}* {memberPtrReg}, i32 0,{arrayPositions}");
                    
                    string innerType = GetInnerType(memberType);
                    memberPtrReg = arrayPtrReg;
                    registerTypes[arrayPtrReg] = innerType + "*";
                }

                bool nextIsPointerAccess = context.structGet().GetText().Contains("->");
                string nextType = memberType.TrimEnd('*');

                if (!nextType.StartsWith("%") && !nextType.StartsWith("i") && !nextType.StartsWith("double") && !nextType.StartsWith("["))
                {
                    nextType = "%" + nextType;
                }

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
                    
                    bool isAlreadyPointer = registerTypes.ContainsKey(currentRegister) && registerTypes[currentRegister].EndsWith("*");
                    
                    if (isPointerAccess && !isAlreadyPointer)
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
                    
                    registerTypes[memberPtrReg] = memberType + "*";
                }
                else if (heterogeneousType is UnionType unionType)
                {
                    memberType = GetMemberType(heterogeneousType, memberId);

                    memberPtrReg = nextRegister();
                    
                    string bitcastExpr = unionType.GetLLVMVar(memberId, currentRegister);
                    currentBody.AppendLine($"   {memberPtrReg} = {bitcastExpr}");
                    registerTypes[memberPtrReg] = memberType + "*";
                }
                else
                {
                    throw new Exception($"Unknown heterogeneous type");
                }

                if (indexes.Length > 0)
                {
                    string arrayPositions = CalculateArrayPosition(indexes);
                    string arrayPtrReg = nextRegister();
                    currentBody.AppendLine($"   {arrayPtrReg} = getelementptr inbounds {memberType}, {memberType}* {memberPtrReg}, i32 0,{arrayPositions}");
                    
                    string innerType = GetInnerType(memberType);
                    memberPtrReg = arrayPtrReg;
                    registerTypes[arrayPtrReg] = innerType + "*";
                }

                return memberPtrReg;
            }
        }

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
            
            if (!registerTypes.ContainsKey(register))
            {
                throw new Exception($"Register type not found for {register}");
            }
            
            string registerType = registerTypes[register];
            var currentBody = getCurrentBody();

            string actualType = registerType.EndsWith("*") 
                ? registerType.Substring(0, registerType.Length - 1) 
                : registerType;

            if (actualType == "[256 x i8]")
            {
                string ptrReg = nextRegister();
                currentBody.AppendLine($"   {ptrReg} = getelementptr inbounds [256 x i8], [256 x i8]* {register}, i32 0, i32 0");
                registerTypes[ptrReg] = "i8*";
                return ptrReg;
            }

            string loadReg = nextRegister();
            currentBody.AppendLine($"   {loadReg} = load {actualType}, {registerType} {register}");
            registerTypes[loadReg] = actualType;
            return loadReg;
        }
    }
}