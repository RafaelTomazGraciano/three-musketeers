namespace Three_Musketeers.Utils
{
    public static class CastTypes
    {
        public static bool TwoTypesArePermitedToCast(string type1, string type2)
        {
            // Normalize for easier comparison
            type1 = type1.Trim();
            type2 = type2.Trim();

            //  handle pointer types ---
            bool isPointer1 = type1.Contains("*") || type1 == "pointer";
            bool isPointer2 = type2.Contains("*") || type2 == "pointer";

            // If both are pointers of any level, allow assignment
            if (isPointer1 && isPointer2)
                return true;

            // If one is pointer and the other is int (common in malloc assignments), allow it
            if ((isPointer1 && type2 == "int") || (isPointer2 && type1 == "int"))
                return true;

            // --- existing primitive type rules ---
            bool anyIsDouble = type1 == "double" || type2 == "double";
            bool anyIsChar = type1 == "char" || type2 == "char";
            bool anyIsInt = type1 == "int" || type2 == "int";
            bool anyIsBool = type1 == "bool" || type2 == "bool";
            bool anyIsString = type1 == "string" || type2 == "string";

            if (type1 == type2) return true;
            if (anyIsDouble && (anyIsChar || anyIsInt || anyIsBool)) return true;
            if (anyIsInt && (anyIsChar || anyIsBool)) return true;
            if (anyIsChar && (anyIsBool || !anyIsString)) return true;

            return false;
        }
    }
}