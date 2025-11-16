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
            
            // Remove trailing comma
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
            string varName = context.ID().GetText();
            string llvmType = getLLVMType(type);
            if (type == "string") llvmType = "[256 x i8]";
            
            // FIX: Check for pointer declarations
            var pointerTokens = context.POINTER();
            int pointerLevel = pointerTokens?.Length ?? 0;
            
            // If the type is a struct name (like "Node"), get its LLVM name
            if (structsTypes.ContainsKey(type))
            {
                llvmType = structsTypes[type].GetLLVMName();
            }
            
            // Add pointer markers
            for (int i = 0; i < pointerLevel; i++)
            {
                llvmType += "*";
            }
            
            // Process array dimensions
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
            
            if (!registerTypes.ContainsKey(register))
            {
                throw new Exception($"Register type not found for {register}");
            }
            
            // register is already a pointer to the field 
            string fieldPtrType = registerTypes[register];  
            
            // Remove the pointer to get the actual field type
            string fieldType = fieldPtrType.TrimEnd('*'); 
            
            string expr = visit(context.expr());
            
            // Convert 0 to null for pointer fields
            if (fieldType.Contains('*') && expr == "0")
            {
                expr = "null";
            }
            
            // Get the actual type of what we're storing
            string valueType = registerTypes.ContainsKey(expr) ? registerTypes[expr] : fieldType;

            // Type conversion for struct pointers if needed
            if (fieldType.StartsWith("%") && fieldType.EndsWith("*") && 
                valueType.StartsWith("%") && valueType.EndsWith("*") && 
                fieldType != valueType)
            {
                string bitcastReg = nextRegister();
                currentBody.AppendLine($"   {bitcastReg} = bitcast {valueType} {expr} to {fieldType}");
                expr = bitcastReg;
                valueType = fieldType;  // Update valueType after bitcast
            }

            // Handle string copying to char arrays
            if (fieldType == "[256 x i8]" && valueType == "i8*")
            {
                string destPtr = nextRegister();
                currentBody.AppendLine($"   {destPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {register}, i32 0, i32 0");
                string strcpyResult = nextRegister();
                currentBody.AppendLine($"   {strcpyResult} = call i8* @strcpy(i8* {destPtr}, i8* {expr})");
            }
            else
            {
                // Store the value into the field pointer
                currentBody.AppendLine($"   store {valueType} {expr}, {fieldPtrType} {register}");
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
                        // For direct pointer params, the register already holds the pointer value
                        currentRegister = pointerReg;
                        currentType = baseStructType;  // Keep the % prefix
                        registerTypes[pointerReg] = pointerType;
                    }
                    else
                    {
                        // Load the pointer value from the pointer variable
                        string loadedPtrReg = nextRegister();
                        currentBody.AppendLine($"   {loadedPtrReg} = load {pointerType}, {pointerType}* {pointerReg}");
                        
                        currentRegister = loadedPtrReg;
                        currentType = baseStructType; 
                        registerTypes[loadedPtrReg] = pointerType;  // The loaded register contains a pointer
                    }
                }
                else if (exprContext is ExprParser.VarStructContext varStructCtx)
                {
                    // Handle (*structExpr.member).field
                    string innerStructReg = VisitStructGet(varStructCtx.structGet());
                    string innerStructType = registerTypes[innerStructReg];
                    
                    // Remove the * to get the base type
                    string innerBaseType = CodeGeneratorBase.RemoveOneAsterisk(innerStructType);
                    
                    // Load the pointer value
                    string loadedPtrReg = nextRegister();
                    currentBody.AppendLine($"   {loadedPtrReg} = load {innerBaseType}, {innerStructType} {innerStructReg}");
                    
                    currentRegister = loadedPtrReg;
                    currentType = CodeGeneratorBase.RemoveOneAsterisk(innerBaseType);  // Remove one more *
                    registerTypes[loadedPtrReg] = innerBaseType;
                }
                else if (exprContext is ExprParser.DerrefExprContext derrefExprCtx)
                {
                    // Handle nested dereferences: (*(*pp))
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

                // Check if this is a pointer type 
                bool isPointerType = pointerType.EndsWith("*");
                
                if (isPointerType)
                {
                    // This is a pointer to a struct
                    string baseType = CodeGeneratorBase.RemoveOneAsterisk(pointerType);
                    
                    if (variable.isDirectPointerParam)
                    {
                        currentRegister = pointerReg;
                        currentType = baseType;
                    }
                    else
                    {
                        // Load the pointer value
                        string loadedPtrReg = nextRegister();
                        currentBody.AppendLine($"   {loadedPtrReg} = load {pointerType}, {pointerType}* {pointerReg}");
                        
                        currentRegister = loadedPtrReg;
                        currentType = baseType;
                        registerTypes[loadedPtrReg] = pointerType;
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
                        // Pointer indexing
                        string indexExpr = visit(indexes[0].expr());
                        string gepReg = nextRegister();
                        
                        // Use pointer arithmetic 
                        currentBody.AppendLine($"   {gepReg} = getelementptr inbounds {currentType}, {currentType}* {currentRegister}, i32 {indexExpr}");
                        
                        currentRegister = gepReg;
                        registerTypes[gepReg] = currentType + "*";
                    }
                    else
                    {
                        // Array indexing
                        string arrayPositions = CalculateArrayPosition(indexes);
                        string ptrReg = nextRegister();
                        
                        string varLLVMType = variable.LLVMType;
                        currentBody.AppendLine($"   {ptrReg} = getelementptr inbounds {varLLVMType}, {varLLVMType}* {currentRegister}, i32 0,{arrayPositions}");
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
                // This is a primitive type or pointer
                if (!registerTypes.ContainsKey(currentRegister))
                {
                    // The register is a pointer to this primitive type
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

            // Remove ONE asterisk to get the actual type we're loading
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