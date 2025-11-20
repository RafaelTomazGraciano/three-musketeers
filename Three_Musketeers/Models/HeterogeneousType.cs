namespace Three_Musketeers.Models
{
    public abstract class HeterogenousType
    {
        protected string LLVMName;

        protected List<HeterogenousMember> members;

        public int totalSize { get; set; }

        public HeterogenousType(string LLVMName, List<HeterogenousMember> members)
        {
            this.LLVMName = LLVMName;
            this.members = members;
        }

        public abstract string GetLLVMVar(string name, string ptr);

        public string GetLLVMName() { return LLVMName; }

        public List<HeterogenousMember> GetMembers()
        {
            return members;
        }
    }
}