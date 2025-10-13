namespace Three_Musketeers.Models
{
    public class Scope
    {
        public Dictionary<string, Symbol> symbols { get; }
            public Scope? parent { get; }

            public Scope(Scope? parent = null)
            {
                this.symbols = new Dictionary<string, Symbol>();
                this.parent = parent;
            }
    }
}