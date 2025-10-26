using Antlr4.Runtime.Misc;

namespace Three_Musketeers.Models
{
    public class StructModel
    {
        public string LLVMTypeName;
        public List<Variable> args;

        public int size;

        public StructModel(string LLVMTypeName)
        {
            this.LLVMTypeName = LLVMTypeName;
            args = [];
            size = 0;
        }

        public int GetFieldIndex(string fieldName)
        {
            return args.FindIndex(x => x.name == fieldName);
        }

        public string GetFieldType(string fieldName)
        {
            return args.Find(x => x.name == fieldName)!.LLVMType;
        }

        public void AddSize(int v)
        {
            size += v;
        }
    }
}