namespace Three_Musketeers.Models
{
    public class ArrayVariable : Variable
    {
        public int size { get; }
        public string innerType;
        public ArrayVariable(string name, string type, string llvmType, string register, int size, string innerType)
            : base(name, type, llvmType, register)
        {
            this.size = size;
            this.innerType = innerType;
        }
    }
}