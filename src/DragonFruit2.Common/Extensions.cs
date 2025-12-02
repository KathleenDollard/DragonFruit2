using System.Runtime.CompilerServices;
using System.Text;

namespace DragonFruit2.Common;

public static class Extensions
{
    extension(string s)
    {
        public string ToKebabCase()
        {
            if (string.IsNullOrEmpty(s)) return s;
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (char.IsUpper(c))
                {
                    if (i > 0 && (char.IsLower(s[i - 1]) || char.IsDigit(s[i - 1])))
                        sb.Append('-');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().ToLower();
        }

        public string ToCamelCase()
        {
            if (string.IsNullOrEmpty(s)) return s;

            // If first char is not uppercase, already camel-case-ish
            if (!char.IsUpper(s[0])) return s;

            var chars = s.ToCharArray();

            // Count leading uppercase run
            int run = 0;
            while (run < chars.Length && char.IsUpper(chars[run])) run++;

            if (run == 1)
            {
                // Just lowercase the first character: "Name" -> "name"
                chars[0] = char.ToLowerInvariant(chars[0]);
                return new string(chars);
            }

            if (run == chars.Length)
            {
                // All uppercase: "XML" -> "xml"
                return s.ToLowerInvariant();
            }

            // Mixed e.g. "XMLHttp" -> lowercase all but the last uppercase in the run: "xmlHttp"
            for (int i = 0; i < run - 1; i++)
                chars[i] = char.ToLowerInvariant(chars[i]);

            return new string(chars);
        }
    }
}
