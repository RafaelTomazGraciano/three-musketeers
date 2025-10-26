namespace Three_Musketeers.Models
{
    public class Symbol
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool isInitializated { get; set; }
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
        public int pointerLevel { get; } // Nível de indireção para arrays de ponteiros

        public ArraySymbol(string name, string innerType, int line, List<int> dimensions, int pointerLevel = 0)
            : base(name, "array", line)
        {
            this.innerType = innerType;
            this.dimensions = dimensions;
            this.pointerLevel = pointerLevel;
        }

        public override string ToString()
        {
            string dims = string.Join("][", dimensions);
            string pointers = new string('*', pointerLevel);
            return $"{type} {name}[{dims}] of {innerType}{pointers} (line {line})";
        }

        // Verifica se é um array de ponteiros
        public bool IsPointerArray()
        {
            return pointerLevel > 0;
        }

        // Verifica se é um array de structs
        public bool IsStructArray()
        {
            return innerType.StartsWith("struct_") || innerType == "struct";
        }
    }

    public class PointerSymbol : Symbol
    {
        public string innerType { get; set; }
        public bool isDynamic { get; set; }
        public int amountOfPointers { get; set; }
        public List<int>? arrayDimensions { get; set; } // Para ponteiros que são arrays

        public PointerSymbol(string name, string innerType, int line, int amountOfPointers,
                           bool isDynamic = false, List<int>? arrayDimensions = null)
            : base(name, "pointer", line)
        {
            this.innerType = innerType;
            this.amountOfPointers = amountOfPointers;
            this.isDynamic = isDynamic;
            this.arrayDimensions = arrayDimensions;
        }

        public override string ToString()
        {
            string pointers = new string('*', amountOfPointers);
            string dims = arrayDimensions != null ? $"[{string.Join("][", arrayDimensions)}]" : "";
            string dynamic = isDynamic ? " (dynamic)" : "";
            return $"{type} {innerType}{pointers} {name}{dims}{dynamic} (line {line})";
        }

        // Verifica se é um array de ponteiros
        public bool IsPointerArray()
        {
            return arrayDimensions != null && arrayDimensions.Count > 0;
        }
    }

    public class StructSymbol : Symbol
    {
        private readonly Dictionary<string, Symbol> symbols;
        public string structName { get; set; } // Nome do tipo struct

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