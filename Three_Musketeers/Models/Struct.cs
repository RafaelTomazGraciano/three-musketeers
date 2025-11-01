using Antlr4.Runtime.Misc;

namespace Three_Musketeers.Models
{
    public class StructModel : HeterogenousType
    {

        public StructModel(string LLVMName, List<Variable> members, Func<string, int> GetSize) : base(LLVMName, members)
        {
            totalSize = 0;
            foreach (var member in members)
            {
                totalSize += GetSize(member.type);
            }
        }

        public int GetFieldIndex(string fieldName)
        {
            return members.FindIndex(x => x.name == fieldName);
        }

        public string GetFieldType(string fieldName)
        {
            return members.Find(x => x.name == fieldName)!.LLVMType;
        }

        public void AddSize(int v)
        {
            totalSize += v;
        }

        public override string GetLLVMVar(string name, string ptr)
        {
            int index = GetFieldIndex(name);
            return $"getelement {LLVMName}, {LLVMName}* {ptr}, i32 0, i32 {index}";
        }
    }
}