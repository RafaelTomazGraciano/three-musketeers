namespace Three_Musketeers.Models
{
    public class Symbol
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool isInitializated { get; set; }
        public int line { get; set; } //line declarated
        public bool isConstant { get; set; } 
        public string? constantValue { get; set; }

        public Symbol(string name, string type, int line)
        {
            this.name = name;
            this.type = type;
            this.line = line;
            this.isInitializated = false;
            this.isConstant = false;
            this.constantValue = null;
        }

        //for defines
        public Symbol(string name, string type, int line, string constantValue)
        {
            this.name = name;
            this.type = type;
            this.line = line;
            this.isInitializated = true;
            this.isConstant = true;
            this.constantValue = constantValue;
        }

        public override string ToString()
        {
            if (isConstant)
            {
                return $"{type} {name} = {constantValue} [CONSTANT] (line {line})";
            }
            return $"{type} {name} (line {line})";
        }
    }

    public class ArraySymbol : Symbol
    {
        public string elementType { get; }
        public List<int> Dimensions { get; }
        public int pointerLevel { get; }

        public ArraySymbol(string name, string elementType, int line, List<int> Dimensions, int pointerLevel = 0)
            : base(name, "array", line)
        {
            this.elementType = elementType;
            this.Dimensions = Dimensions;
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

    public class PointerSymbol : Symbol
    {
        public string pointeeType { get; set; }  // What the pointer points to
        public int pointerLevel { get; set; }    // Number of * (indirection level)
        public bool IsDynamic { get; set; }

        public PointerSymbol(string name, string pointeeType, int line, int pointerLevel = 1, bool isDynamic = false)
            : base(name, "pointer", line)
        {
            this.pointeeType = pointeeType;
            this.pointerLevel = pointerLevel;
            IsDynamic = isDynamic;
        }

        public override string ToString()
        {
            string pointers = new string('*', pointerLevel);
            string dynamic = isDynamic ? " (dynamic)" : "";
            return $"{pointeeType}{pointers} {name}{dynamic} (line {line})";
        }

        // Returns the LLVM type
        // Example: int** -> i32**
        public string GetLLVMType(Func<string, string> getLLVMType)
        {
            string baseType = getLLVMType(pointeeType);
            return baseType + new string('*', pointerLevel);
        }
    }

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