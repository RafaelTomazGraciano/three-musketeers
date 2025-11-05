namespace Three_Musketeers.Models
{
    public class HeterogenousMember
    {
        public string name;
        public string LLVMType;
        public HeterogenousMember(string name, string LLVMType)
        {
            this.name = name;
            this.LLVMType = LLVMType;
        }
    }
}