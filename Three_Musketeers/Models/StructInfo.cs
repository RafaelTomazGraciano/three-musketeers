
namespace Three_Musketeers.Models
{
    public class StructInfo : HeterogenousInfo
    {
        public StructInfo(string name, Dictionary<string, Symbol> members, int line) : base(name, members, line)
        {
        }
    }
}