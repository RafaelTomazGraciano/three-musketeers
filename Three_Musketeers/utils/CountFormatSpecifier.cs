namespace Three_Musketeers.Utils
{
    public static class CountFormatSpecifier
    {
        public static int Count(string formatString)
        {
            int count = 0;
            for (int i = 0; i < formatString.Length - 1; i++)
            {
                if (formatString[i] == '%' && formatString[i + 1] != '%')
                {
                    char next = formatString[i + 1];
                    if ("difcs".Contains(next))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}