using System.Text;

namespace Three_Musketeers.Utils
{
    public static class EscapeSequenceProcessor
    {
        public static (string processed, int byteCount) Process(string str)
        {
            var result = new StringBuilder();
            int byteCount = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\\' && i + 1 < str.Length)
                {
                    char next = str[i + 1];
                    switch (next)
                    {
                        case 'n':
                            result.Append("\\0A"); //newline
                            byteCount++;
                            i++;
                            break;
                        case 't':
                            result.Append("\\09"); // tab
                            byteCount++;
                            i++;
                            break;
                        case 'r':
                            result.Append("\\0D"); //carriage return
                            byteCount++;
                            i++;
                            break;
                        case '\\':
                            result.Append("\\\\"); //backslash
                            byteCount++;
                            i++;
                            break;
                        case '"':
                            result.Append("\\22"); //quotes
                            byteCount++;
                            i++;
                            break;
                        case '0':
                            result.Append("\\00"); //null
                            byteCount++;
                            i++;
                            break;
                        default:
                            result.Append("\\");
                            result.Append(next);
                            byteCount += 2;
                            i++;
                            break;
                    }
                }
                else
                {
                    result.Append(str[i]);
                    byteCount++;
                }
            }
            return (result.ToString(), byteCount);
        }
    }
}