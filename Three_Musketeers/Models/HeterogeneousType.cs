namespace Three_Musketeers.Models
{
    public abstract class HeterogenousType
    {
        protected string LLVMName;

        protected List<HeterogenousMember> members;

        protected int totalSize;

        public HeterogenousType(string LLVMName, List<HeterogenousMember> members)
        {
            this.LLVMName = LLVMName;
            this.members = members;
        }

        public abstract string GetLLVMVar(string name, string ptr);

        public string GetLLVMName() { return LLVMName; }

        public int GetTotalSize()
        {
            return totalSize;
        }

        public List<HeterogenousMember> GetMembers()
        {
            return members;
        }
    }
}