using System.Collections.Generic;
using Three_Musketeers.Models;

namespace Three_Musketeers.Utils
{
    public static class FormatSpecifierParser
    {
        public static List<FormatSpecifier> Parse(string formatString)
        {
            var specifiers = new List<FormatSpecifier>();

            for (int i = 0; i < formatString.Length; i++){
                if (formatString[i] == '%'){
                    if (i + 1 >= formatString.Length){
                        break;
                    }
                    if (formatString[i + 1] == '%'){
                        i++;
                        continue;
                    }
                    i++;

                    int? precision = null;

                    while (i < formatString.Length && "+-0 #".Contains(formatString)){
                        i++;
                    }
                    while (i < formatString.Length && char.IsDigit(formatString[i])){
                        i++;
                    }
                    if (i < formatString.Length && formatString[i] == '.'){
                        i++;
                        int precisionValue = 0;
                        while (i < formatString.Length && char.IsDigit(formatString[i])){
                            precisionValue = precisionValue * 10 + (formatString[i] - '0');
                            i++;
                        }
                        precision = precisionValue;
                    }
                    while (i < formatString.Length && "lhLzjt".Contains(formatString)){
                        i++;
                    }
                    if (i < formatString.Length){
                        char type = formatString[i];
                        specifiers.Add(new FormatSpecifier(type, precision));
                    }
                }
            }
            return specifiers;
        }
    }
}