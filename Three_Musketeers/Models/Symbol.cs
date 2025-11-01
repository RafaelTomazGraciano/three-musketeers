namespace Three_Musketeers.Models
{
    public class Symbol
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool isInitializated { get; set; }
        public int line { get; set; }

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
        public string elementType { get; }  // Changed from innerType for clarity
        public List<int> dimensions { get; }
        public int pointerLevel { get; }  // How many * after the element type

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

        // Returns the LLVM type for the element (with pointers)
        public string GetElementLLVMType(Func<string, string> getLLVMType)
        {
            string baseType = getLLVMType(elementType);
            return baseType + new string('*', pointerLevel);
        }

        // Returns the full array LLVM type
        // Example: int*[10][5] -> [10 x [5 x i32*]]
        public string GetArrayLLVMType(Func<string, string> getLLVMType)
        {
            string elementLLVM = GetElementLLVMType(getLLVMType);
            string arrayType = elementLLVM;
            
            // Build from innermost to outermost dimension
            for (int i = dimensions.Count - 1; i >= 0; i--)
            {
                arrayType = $"[{dimensions[i]} x {arrayType}]";
            }
            
            return arrayType;
        }

        public bool IsPointerArray()
        {
            return pointerLevel > 0;
        }

        public bool IsStructArray()
        {
            return elementType.StartsWith("struct_") || elementType == "struct";
        }

        public bool IsStringArray()
        {
            return elementType == "string";
        }
    }

    public class PointerSymbol : Symbol
    {
        public string pointeeType { get; set; }  // What the pointer points to
        public int pointerLevel { get; set; }    // Number of * (indirection level)
        public bool isDynamic { get; set; }

        public PointerSymbol(string name, string pointeeType, int line, int pointerLevel = 1, bool isDynamic = false)
            : base(name, "pointer", line)
        {
            this.pointeeType = pointeeType;
            this.pointerLevel = pointerLevel;
            this.isDynamic = isDynamic;
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