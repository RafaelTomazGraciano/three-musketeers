
namespace Three_Musketeers.Models
{
    public class Union : HeterogenousType
    {
        public Union(string LLVMName, List<Variable> members, Func<string, int> getSize) : base(LLVMName, members)
        {
            totalSize = 0;
            foreach (var member in members)
            {
                int size = getSize(member.type);
                if (size > totalSize) {
                    totalSize = size;
                    LLVMName = member.name;
                }
            }
        }

        public override string GetLLVMVar(string name, string ptr)
        {
            members.First(mbr => mbr.name == name);
            return "";
        }
    }
}