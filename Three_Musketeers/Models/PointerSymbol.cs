namespace Three_Musketeers.Models
{
    public class PointerSymbol : Symbol
    {
        public string innerType;

        public bool isDynamic;

        public int amountOfPointers;

        public PointerSymbol(string name, string innerType, int line, int amountOfPointers, bool isDynamic = false)
        : base(name, "pointer", line)
        {
            this.innerType = innerType;
            this.amountOfPointers = amountOfPointers;
            this.isDynamic = isDynamic;
        }
    }
}