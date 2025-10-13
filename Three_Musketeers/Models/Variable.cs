namespace Three_Musketeers.Models
{
    public class Variable
    {
        public string name { get; set; }
        public string type { get; set; }
        public string LLVMType { get; set; }
        public string register { get; set; }

        public Variable(string name, string type, string llvmType, string register)
        {
            this.name = name;
            this.type = type;
            this.LLVMType = llvmType;
            this.register = register;
        }
    }

    public class ArrayVariable : Variable
    {
        public int size { get; }
        public List<int> dimensions { get; }
        public string innerType;
        public ArrayVariable(string name, string type, string llvmType, string register, int size, List<int> dimensions, string innerType)
            : base(name, type, llvmType, register)
        {
            this.size = size;
            this.dimensions = dimensions;
            this.innerType = innerType;
        }
    }
}