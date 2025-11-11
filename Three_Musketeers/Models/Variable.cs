namespace Three_Musketeers.Models
{
    public class Variable
    {
        public string name { get; set; }
        public string type { get; set; }
        public string LLVMType { get; set; }
        public string register { get; set; }
        public bool isDirectPointerParam = false;

        public Variable(string name, string type, string LLVMType, string register, bool isDirectPointerParam = false)
        {
            this.name = name;
            this.type = type;
            this.LLVMType = LLVMType;
            this.register = register;
            this.isDirectPointerParam = isDirectPointerParam;
        }
    }
}