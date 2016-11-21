using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Justin.AspNet.ModAuthPubTkt
{
    internal static class ConfigurationExtensons
    {
        public static string GetValue(this NameValueCollection config, string key, string defaultValue = null)
        {
            var val = config.Get(key);
            if (string.IsNullOrEmpty(val))
            {
                return defaultValue;
            }
            return val;
        }
    }
}
