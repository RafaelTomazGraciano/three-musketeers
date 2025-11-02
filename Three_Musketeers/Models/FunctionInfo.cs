namespace Three_Musketeers.Models
{
    public class FunctionInfo
    {
        public string? returnType { get; set; }
        public int returnPointerLevel { get; set; } = 0;
        public List<(string Type, string Name, int PointerLevel)>? parameters { get; set; }
        public bool isVoid => returnType == "void";
        public bool hasReturnStatement { get; set; }
        
        public string GetFullReturnType()
        {
            if (isVoid)
                return "void";
                
            string pointers = new string('*', returnPointerLevel);
            return returnType ?? "unknown";
        }

        public override string ToString()
        {
            string paramsList = parameters != null && parameters.Count > 0
                ? string.Join(", ", parameters.Select(p => $"{p.Type}{new string('*', p.PointerLevel)} {p.Name}"))
                : "";
            return $"{GetFullReturnType()} ({paramsList})";
        }
    }
}