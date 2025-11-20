namespace Three_Musketeers.Models
{
    public class StructType : HeterogenousType
    {

        public StructType(string LLVMName, List<HeterogenousMember> members, Func<string, int> GetSize) : base(LLVMName, members)
        {
            // it's calculated properly in GetSize() when needed
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