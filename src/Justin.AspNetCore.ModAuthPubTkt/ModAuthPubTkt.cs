using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Justin.AspNetCore.ModAuthPubTkt 
{
    public struct ModAuthPubTkt
    {
        private readonly string _value;

        private readonly IDictionary<string, string> _parsed;

        private string GetValue(string key, string defaultValue)
        {
            string s;
            if (_parsed.TryGetValue("uid", out s))
            {
                return s;
            }
            else
            {
                return defaultValue;
            }
        }

        public ModAuthPubTkt(string value)
        {
            _value = value;
            _parsed = Utils.ParseTicket(value);
        }

        public string UserID 
        {
            get
            {
               return GetValue("uid", null);
            }
        }

        public DateTimeOffset ValidUntil
        {
            get
            {
                long val = long.Parse(GetValue("validuntil", "0"));

                return ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(val);
            }
        }

        public DateTimeOffset GracePeriod
        {
            get
            {
                long val = long.Parse(GetValue("graceperiod", "0"));

                return ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(val);
            }
        }

        public string ClientIP
        {
            get
            {
                return GetValue("cip", null);
            }
        }

        public IEnumerable<string> Tokens
        {
            get
            {
                var val = GetValue("tokens", null);

                if (val == null)
                {
                    return null;
                }

                return val.Split(',');
            }
        }

        public string UserData
        {
            get
            {
                return GetValue("udata", null);
            }
        }

        public string BasicAuth
        {
            get
            {
                return GetValue("bauth", null);
            }
        }

        public byte[] Signature
        {
            get
            {
                var sig = GetValue("sig", null);

                if (sig == null)
                {
                    return null;
                }

                return Convert.FromBase64String(sig);
            }
        }

        public string SignedString
        {
            get
            {
                var idx = _value.IndexOf(";sig=");

                return _value.Substring(0, idx);
            }
        }
    }
}