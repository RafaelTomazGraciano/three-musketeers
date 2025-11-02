namespace Three_Musketeers.Models
{
    public class ArraySymbol : Symbol
    {
        public string elementType { get; }
        public List<int> dimensions { get; }
        public int pointerLevel { get; }

        public ArraySymbol(string name, string elementType, int line, List<int> dimensions, int pointerLevel = 0)
            : base(name, "array", line)
        {
            this.elementType = elementType;
            this.dimensions = dimensions;
            this.pointerLevel = pointerLevel;
        }

        public override string ToString()
        {
            string dims = string.Join("][", dimensions);
            string pointers = new string('*', pointerLevel);
            return $"{type} {name}[{dims}] of {elementType}{pointers} (line {line})";
        }

        public string GetElementLLVMType(Func<string, string> getLLVMType)
        {
            string baseType = getLLVMType(elementType);
            return baseType + new string('*', pointerLevel);
        }
    
    }
}