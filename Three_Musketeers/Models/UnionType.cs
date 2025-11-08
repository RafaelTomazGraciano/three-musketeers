namespace Three_Musketeers.Models
{
    public class UnionType : HeterogenousType
    {
        private string largestMemberType; 
        
        public UnionType(string LLVMName, List<HeterogenousMember> members, Func<string, int> getSize) : base(LLVMName, members)
        {
            totalSize = 0;
            largestMemberType = members[0].LLVMType; 
            
            foreach (var member in members)
            {
                int size = getSize(member.LLVMType);
                if (size > totalSize) 
                {
                    totalSize = size;
                    largestMemberType = member.LLVMType; 
                }
            }
        }

        public string GetLargestMemberType()
        {
            return largestMemberType;
        }

        public override string GetLLVMVar(string name, string ptr)
        {
            var member = members.First(mbr => mbr.name == name);
            return $"bitcast {LLVMName}* {ptr} to {member.LLVMType}*";
        }
    }
}