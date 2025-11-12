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
                            result.Append("\\5C"); //backslash
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
                    char c = str[i];
                    
                    // Check if character is multi-byte UTF-8
                    int charByteCount = GetUtf8ByteCount(c);
                    
                    if (charByteCount == 1)
                    {
                        // ASCII character - use as is
                        result.Append(c);
                        byteCount += 1;
                    }
                    else
                    {
                        // Multi-byte UTF-8 character - encode as hex escape sequences
                        byte[] utf8Bytes = Encoding.UTF8.GetBytes(new char[] { c });
                        foreach (byte b in utf8Bytes)
                        {
                            result.Append($"\\{b:X2}");
                            byteCount += 1;
                        }
                    }
                }
            }
            
            return (result.ToString(), byteCount);
        }
        
        private static int GetUtf8ByteCount(char c)
        {
            // Get the UTF-8 byte count for a character
            return Encoding.UTF8.GetByteCount(new char[] { c });
        }
    }
}