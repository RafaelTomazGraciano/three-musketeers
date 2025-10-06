namespace Three_Musketeers.Models
{
    public class FunctionInfo
    {
        public string? returnType { get; set; }
        public List<int>? returnDimensions { get; set; }
        public List<(string Type, string Name)>? parameters { get; set; }
        public bool isVoid => returnType == "void";
        public bool isArray => returnDimensions != null && returnDimensions.Count > 0;
        public bool hasReturnStatement { get; set; }
        
        public string GetFullReturnType()
        {
            if (isVoid)
                return "void";
                
            if (!isArray)
                return returnType ?? "unknown";
            
            string dims = string.Join("][", returnDimensions!);
            return $"{returnType}[{dims}]";
        }

        public override string ToString()
        {
            string paramsList = parameters != null && parameters.Count > 0
                ? string.Join(", ", parameters)
                : "";
            return $"{GetFullReturnType()} ({paramsList})";
        }
    }
}