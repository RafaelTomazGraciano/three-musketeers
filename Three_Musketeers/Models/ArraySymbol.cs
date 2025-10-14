namespace Three_Musketeers.Models
{
    public class ArraySymbol : Symbol
    {
        public string innerType { get; }
        public List<int> dimensions { get; }

        public ArraySymbol(string name, string innerType, int line, List<int> dimensions)
            : base(name, "array", line)
        {
            this.innerType = innerType;
            this.dimensions = dimensions;
        }

        public override string ToString()
        {
            string dims = string.Join("][", dimensions);
            return $"{type} {name}[{dims}] (line {line})";
        }
    }
}