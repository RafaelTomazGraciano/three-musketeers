namespace Three_Musketeers.Models
{
    public class StructInfo
    {
        public string name { get; set; }
        public Dictionary<string, Symbol> members { get; set; }
        public int line { get; set; }

        public StructInfo(string name, Dictionary<string, Symbol> members, int line)
        {
            this.name = name;
            this.members = members;
            this.line = line;
        }

        public Symbol? GetMember(string memberName)
        {
            return members.ContainsKey(memberName) ? members[memberName] : null;
        }

        public bool HasMember(string memberName)
        {
            return members.ContainsKey(memberName);
        }
    }
}