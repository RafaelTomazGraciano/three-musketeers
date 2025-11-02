namespace Three_Musketeers.Models
{
    public class StructSymbol : Symbol
    {
        private readonly Dictionary<string, Symbol> symbols;
        public string structName { get; set; }

        public Symbol? GetSymbol(string name)
        {
            return symbols.ContainsKey(name) ? symbols[name] : null;
        }

        public Dictionary<string, Symbol> GetAllSymbols()
        {
            return new Dictionary<string, Symbol>(symbols);
        }

        public StructSymbol(string name, string structName, int line, Dictionary<string, Symbol> symbols)
            : base(name, $"struct_{structName}", line)
        {
            this.structName = structName;
            this.symbols = symbols;
        }

        public override string ToString()
        {
            return $"struct {structName} {name} (line {line})";
        }
    }
}