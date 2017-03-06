using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Justin.ModAuthPubTkt
{
    public static class DictionaryExtensions
    {
        public static IDictionary<string, string> FromBase64(this string value, IEqualityComparer<string> stringComparer = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (stringComparer == null)
            {
                stringComparer = StringComparer.OrdinalIgnoreCase;
            }

            byte[] rawBytes = Convert.FromBase64String(value);
            value = System.Text.UTF8Encoding.UTF8.GetString(rawBytes);

            string[] elements = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] pair;
            var d = new Dictionary<string, string>(stringComparer);

            foreach (var element in elements)
            {
                pair = element.Split(new char[] { '=' }, 2);
                d.Add(pair[0], pair[1]);    
            }

            return d;
        }

        public static string ToBase64(this IDictionary<string, string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var sb = new StringBuilder();

            foreach (var pair in values)
            {
                sb.Append(pair.Key).Append("=").Append(pair.Value).Append(";");
            }

            return Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}
