namespace Three_Musketeers.Models
{
    public class UnionType : HeterogenousType
    {
        private string largestMemberType; 
        
        public UnionType(string LLVMName, List<HeterogenousMember> members, Func<string, int> getSize) : base(LLVMName, members)
        {
            totalSize = 0;
            int largestAlignment = 0;
            largestMemberType = members[0].LLVMType; 
            
            foreach (var member in members)
            {
                int size = getSize(member.LLVMType);
                
                // Calculate alignment for this member
                int memberAlignment;
                if (member.LLVMType == "double")
                {
                    memberAlignment = 8;
                }
                else if (member.LLVMType.Contains('*'))
                {
                    memberAlignment = 8;
                }
                else if (member.LLVMType == "i32")
                {
                    memberAlignment = 4;
                }
                else if (member.LLVMType == "i8")
                {
                    memberAlignment = 1;
                }
                else if (member.LLVMType.StartsWith("%"))
                {
                    // For structs, assume alignment of 4 unless it contains doubles
                    memberAlignment = 4;
                }
                else
                {
                    memberAlignment = 4;
                }
                
                // Choose the largest type, but if sizes are equal, prefer the one with larger alignment
                if (size > totalSize || (size == totalSize && memberAlignment > largestAlignment))
                {
                    totalSize = size;
                    largestAlignment = memberAlignment;
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