namespace Three_Musketeers.Models
{
    public class UnionInfo : HeterogenousInfo
    {
        public UnionInfo(string name, Dictionary<string, Symbol> members, int line) : base(name, members, line)
        {
        }
    }
}