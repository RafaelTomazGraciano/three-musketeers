using System.Collections;
using System.Text;
using Antlr4.Runtime.Tree;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.Struct_Unions
{
    public class UnionCodeGenerator
    {
        private readonly Dictionary<string, HeterogenousType> structsTypes;
        private readonly StringBuilder structDeclaration;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<string, int> getSize;
        
        public UnionCodeGenerator(
            Dictionary<string, HeterogenousType> structsTypes, 
            StringBuilder structDeclaration, 
            Func<string, string> getLLVMType, 
            Func<string, int> getSize)
        {
            this.structsTypes = structsTypes;
            this.structDeclaration = structDeclaration;
            this.getLLVMType = getLLVMType;
            this.getSize = getSize;
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
            
            // Create the UnionType 
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
            var declType = context.type();
            string varName = context.ID().GetText();
            string llvmType;
            
            // Handle "struct TypeName" or "union TypeName" syntax
            if (declType.ChildCount >= 2 && 
                (declType.GetChild(0).GetText() == "struct" || declType.GetChild(0).GetText() == "union"))
            {
                string keyword = declType.GetChild(0).GetText();
                string referencedName = declType.ID().GetText();
                
                if (structsTypes.ContainsKey(referencedName))
                {
                    llvmType = structsTypes[referencedName].GetLLVMName();
                }
                else
                {
                    llvmType = '%' + referencedName;
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
            
            // Check for pointer declarations
            var pointerTokens = context.POINTER();
            int pointerLevel = pointerTokens?.Length ?? 0;
            
            // Add pointer markers
            for (int i = 0; i < pointerLevel; i++)
            {
                llvmType += "*";
            }
            
            // Process array dimensions
            var indexes = context.intIndex();
            if (indexes.Length != 0)
            {
                // Build nested array type from innermost to outermost
                for (int i = indexes.Length - 1; i >= 0; i--)
                {
                    string size = indexes[i].INT().GetText();
                    llvmType = $"[{size} x {llvmType}]";
                }
            }
            
            return llvmType;
        }
    }
}