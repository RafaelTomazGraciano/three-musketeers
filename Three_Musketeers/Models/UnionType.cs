
namespace Three_Musketeers.Models
{
    public class UnionType : HeterogenousType
    {
        public UnionType(string LLVMName, List<HeterogenousMember> members, Func<string, int> getSize) : base(LLVMName, members)
        {
            totalSize = 0;
            foreach (var member in members)
            {
                int size = getSize(member.LLVMType);
                if (size > totalSize) {
                    totalSize = size;
                    LLVMName = member.name;
                }
            }
        }

        public override string GetLLVMVar(string name, string ptr)
        {
            var member = members.First(mbr => mbr.name == name);

            return $"bitcast {ptr} to {member.LLVMType}*";
        }
    }
}