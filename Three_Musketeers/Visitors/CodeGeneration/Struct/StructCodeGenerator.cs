using System.Collections;
using System.Text;
using Antlr4.Runtime.Tree;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Struct
{
    public class StructCodeGenerator
    {
        private readonly Dictionary<string, StructModel> structsTypes;
        private readonly StringBuilder structDeclaration;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<IParseTree, string> visit;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<string, int> getSize;
        public StructCodeGenerator(Dictionary<string, StructModel> structsTypes, StringBuilder structDeclaration, Func<StringBuilder> getCurrentBody, Dictionary<string, string> registerTypes, Func<string> nextRegister, Dictionary<string, Variable> variables, Func<IParseTree, string> visit, Func<string, string> getLLVMType, Func<string, int> getSize)
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
        var declarations = context.declaration();
        string id = context.ID().GetText();
        string structName = $"%{id}";
        structDeclaration.AppendLine($"{structName} = type {{");
        StructModel newStruct = new(structName);

        // Write all declarations with commas between them
        for (int i = 0; i < declarations.Length; i++)
        {
            WriteOnStructDec(declarations[i], newStruct);

            // Add comma if not the last element
            if (i < declarations.Length - 1)
            {
                structDeclaration.Append(',');
            }
            structDeclaration.AppendLine();
        }
    
        structDeclaration.AppendLine("}");
        structsTypes.Add(id, newStruct);
        return null;
    }

        private void WriteOnStructDec(ExprParser.DeclarationContext declaration, StructModel structModel)
        {
            string name;
            string type;
            string llvmType;
            if (declaration is ExprParser.BaseDecContext baseDecContext)
            {
                name = baseDecContext.ID().GetText();
                type = baseDecContext.type().GetText();
                llvmType = getLLVMType(type);
                var indexes = baseDecContext.index();
                List<int> dimensions = [];
                int size = 1;
                foreach (var index in indexes)
                {
                    int intIndex = int.Parse(index.INT().GetText());
                    size *= intIndex;
                    dimensions.Add(intIndex);
                }
                if (type == "string") size *= 256;
                if (size != 1)
                {
                    string innerType = llvmType;
                    llvmType = $"[{size} x {llvmType}]";
                    structModel.args.Add(new ArrayVariable(name, type, llvmType, "", size, dimensions, innerType));
                }
                else
                {
                    structModel.args.Add(new Variable(name, type, llvmType, ""));
                }

                structModel.AddSize(size * getSize(llvmType));
                structDeclaration.Append(llvmType);
                return;
            }
            var pointerDecContext = (ExprParser.PointerDecContext)declaration;
            var pointers = pointerDecContext.POINTER();
            name = pointerDecContext.ID().GetText();
            type = pointerDecContext.type().GetText();
            llvmType = $"{getLLVMType(type)}{new string('*', pointers.Length)}";
            structModel.args.Add(new Variable(name, "pointer", llvmType, ""));
            structDeclaration.Append(llvmType);
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
        // Fix for VisitStructGet method around line 116-144

        public string VisitStructGet(ExprParser.StructGetContext context)
        {
            var currentBody = getCurrentBody();
            string id = context.ID().GetText();
            var indexes = context.index();

            Variable baseVar = variables[id];
            string currentRegister;
            string currentType = baseVar.type;
            string currentLLVMType;

            if (currentType.StartsWith("struct_"))
            {
                currentType = currentType.Substring(7); // Remove "struct_" prefix
            }

            if (indexes.Length > 0)
            {
                currentRegister = HandleArrayIndexing(indexes, baseVar);
                currentLLVMType = baseVar.LLVMType;
            }
            else
            {
                // Check if using -> operator (means baseVar is a pointer to struct)
                bool isArrowOp = context.GetChild(indexes.Length + 1).GetText() == "->";

                if (isArrowOp)
                {
                    currentRegister = nextRegister();
                    // Remove one level of pointer from the type
                    string structType = baseVar.LLVMType.TrimEnd('*');
                    registerTypes[currentRegister] = structType + "*";
                    currentBody.AppendLine($"    {currentRegister} = load {structType}*, {baseVar.LLVMType}* {baseVar.register}");
                    currentLLVMType = structType;
                }
                else
                {
                    // Use the variable's register directly (. operator)
                    currentRegister = baseVar.register;
                    currentLLVMType = getLLVMType(currentType);
                }
            }

            // Navigate through the struct chain
            return NavigateStructChain(context.structContinue(), currentRegister, currentType, currentLLVMType);
        }

        private string NavigateStructChain(ExprParser.StructContinueContext context, string currentRegister, string currentType, string currentLLVMType)
        {
            var currentBody = getCurrentBody();
            string fieldId = context.ID().GetText();
            var indexes = context.index();

            // Clean up the type name if it has "struct_" prefix
            if (currentType.StartsWith("struct_"))
            {
                currentType = currentType.Substring(7);
            }
            
            // Get the struct model for the current type
            StructModel structModel = structsTypes[currentType];
            int fieldIndex = structModel.GetFieldIndex(fieldId);

            Variable fieldVar = structModel.args[fieldIndex];

            // Get pointer to the field using getelementptr
            string fieldPtrReg = nextRegister();
            registerTypes[fieldPtrReg] = fieldVar.LLVMType;
            currentBody.AppendLine($"    {fieldPtrReg} = getelementptr inbounds {currentLLVMType}, {currentLLVMType}* {currentRegister}, i32 0, i32 {fieldIndex}");

            // Handle array indexing on the field if present
            string finalFieldPtr = fieldPtrReg;
            if (indexes.Length > 0)
            {
                finalFieldPtr = HandleFieldArrayIndexing(fieldPtrReg, indexes, fieldVar);
            }

            // Check if there's more navigation (nested struct access)
            var nestedStructGet = context.structGet();
            if (nestedStructGet != null)
            {
                var nestedIndexes = nestedStructGet.index();
                // Determine operator after the nested ID
                bool isArrowOp = nestedStructGet.GetChild(nestedIndexes.Length + 1)?.GetText() == "->";

                if (isArrowOp)
                {
                    // Load the pointer value (field contains a pointer to another struct)
                    string loadedReg = nextRegister();
                    registerTypes[loadedReg] = fieldVar.LLVMType;
                    currentBody.AppendLine($"    {loadedReg} = load {fieldVar.LLVMType}, {fieldVar.LLVMType}* {finalFieldPtr}");
                    return NavigateStructChain(nestedStructGet.structContinue(), loadedReg, fieldVar.type, fieldVar.LLVMType);
                }
                else
                {
                    // Continue with the pointer (. operator - field is a nested struct)
                    return NavigateStructChain(nestedStructGet.structContinue(), finalFieldPtr, fieldVar.type, getLLVMType(fieldVar.type));
                }
            }
            registerTypes[finalFieldPtr] = fieldVar.LLVMType;
            return finalFieldPtr;
        }

        private string HandleArrayIndexing(ExprParser.IndexContext[] indexes, Variable var)
        {
            var currentBody = getCurrentBody();

            if (var is ArrayVariable arrayVar)
            {
                // Calculate flat index for multi-dimensional array
                int flatIndex = 0;
                int multiplier = 1;

                for (int i = indexes.Length - 1; i >= 0; i--)
                {
                    int idx = int.Parse(indexes[i].INT().GetText());
                    flatIndex += idx * multiplier;
                    if (i > 0)
                    {
                        multiplier *= arrayVar.dimensions[i];
                    }
                }

                string elemPtr = nextRegister();
                registerTypes[elemPtr] = arrayVar.innerType + "*";
                currentBody.AppendLine($"    {elemPtr} = getelementptr inbounds {arrayVar.LLVMType}, {arrayVar.LLVMType}* {var.register}, i32 0, i32 {flatIndex}");
                return elemPtr;
        }

        // For pointer-based arrays
        string currentPtr = nextRegister();
        string baseType = var.LLVMType.TrimEnd('*');
        currentBody.AppendLine($"    {currentPtr} = load {var.LLVMType}, {var.LLVMType}* {var.register}");

        foreach (var index in indexes)
        {
            string indexValue = index.INT().GetText();
            string newPtr = nextRegister();
            currentBody.AppendLine($"    {newPtr} = getelementptr inbounds {baseType}, {baseType}* {currentPtr}, i32 {indexValue}");
            currentPtr = newPtr;
        }

        registerTypes[currentPtr] = baseType + "*";
        return currentPtr;
    }

        private string HandleFieldArrayIndexing(string basePtr, ExprParser.IndexContext[] indexes, Variable fieldVar)
        {
            var currentBody = getCurrentBody();

            if (fieldVar is ArrayVariable arrayVar)
            {
                // Calculate flat index for multi-dimensional array
                int flatIndex = 0;
                int multiplier = 1;

                for (int i = indexes.Length - 1; i >= 0; i--)
                {
                    int idx = int.Parse(indexes[i].INT().GetText());
                    flatIndex += idx * multiplier;
                    if (i > 0)
                    {
                        multiplier *= arrayVar.dimensions[i];
                    }
                }

                string elemPtr = nextRegister();
                registerTypes[elemPtr] = arrayVar.innerType + "*";
                currentBody.AppendLine($"    {elemPtr} = getelementptr inbounds {arrayVar.LLVMType}, {arrayVar.LLVMType}* {basePtr}, i32 0, i32 {flatIndex}");
                return elemPtr;
            }

            // For pointer-based arrays in struct fields
            string currentPtr = basePtr;
            string baseType = fieldVar.LLVMType.TrimEnd('*');

            foreach (var index in indexes)
            {
                string indexValue = index.INT().GetText();
                string newPtr = nextRegister();
                currentBody.AppendLine($"    {newPtr} = getelementptr inbounds {baseType}, {baseType}* {currentPtr}, i32 {indexValue}");
                currentPtr = newPtr;
            }

            registerTypes[currentPtr] = baseType + "*";
            return currentPtr;
        }

        public string VisitVarStruct(ExprParser.VarStructContext context)
        {
            var currentBody = getCurrentBody();

            string fieldPtrReg = VisitStructGet(context.structGet());

            string fieldType = registerTypes[fieldPtrReg];
            string resultReg = nextRegister();
            registerTypes[resultReg] = fieldType;
            currentBody.AppendLine($"    {resultReg} = load {fieldType}, {fieldType}* {fieldPtrReg}");
            return resultReg;
        }
    }
}