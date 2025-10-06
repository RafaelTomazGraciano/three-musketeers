namespace Three_Musketeers.Models
{
    public class Symbol
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool isInitializated { get; set; }
        public object? value { get; set; }
        public int line { get; set; } //line declarated

        public Symbol(string name, string type, int line)
        {
            this.name = name;
            this.type = type;
            this.line = line;
            this.isInitializated = false;
        }

        public override string ToString()
        {
            return $"{type} {name} (line {line})";
        }
    }

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