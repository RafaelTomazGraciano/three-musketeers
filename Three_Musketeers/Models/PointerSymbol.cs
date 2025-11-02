namespace Three_Musketeers.Models
{
    public class PointerSymbol : Symbol
    {
        public string pointeeType { get; set; }
        public int pointerLevel { get; set; }
        public bool isDynamic { get; set; }

        public PointerSymbol(string name, string pointeeType, int line, int pointerLevel = 1, bool isDynamic = false)
            : base(name, "pointer", line)
        {
            this.pointeeType = pointeeType;
            this.pointerLevel = pointerLevel;
            isDynamic = isDynamic;
        }

        public override string ToString()
        {
            string pointers = new string('*', pointerLevel);
            string dynamic = isDynamic ? " (dynamic)" : "";
            return $"{pointeeType}{pointers} {name}{dynamic} (line {line})";
        }

        public string GetLLVMType(Func<string, string> getLLVMType)
        {
            string baseType = getLLVMType(pointeeType);
            return baseType + new string('*', pointerLevel);
        }
    }
}