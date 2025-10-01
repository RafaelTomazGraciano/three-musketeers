namespace Three_Musketeers.Models{
    public class FormatSpecifier{
        public char type { get; set; }
        public int? precision { get; set; }
        public string expectedLLVMType { get; set; }

        public FormatSpecifier(char type, int? precision = null){
            this.type = type;
            this.precision = precision;
            expectedLLVMType = GetExpectedType(type);
        }

        private string GetExpectedType(char type){
            return type switch
            {
                'd' or 'i' => "i32",
                'f' => "double",
                'c' => "i32",
                's' => "i8*",
                _ => "i32"
            };
        }

    }
    
}