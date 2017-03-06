using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Justin.ModAuthPubTkt
{
    public class ModAuthPubTkt
    {
        #region Privates

        private readonly string _value;
        private readonly IDictionary<string, string> _parsed;

        #endregion

        #region Constructor

        public ModAuthPubTkt()
        {
            _value = string.Empty;
            _parsed = new Dictionary<string, string>();
        }

        public ModAuthPubTkt(string value)
        {
            _value = value;
            _parsed = Utils.ParseTicket(value);
        }

        #endregion

        #region Private Methods

        private string GetValue(string key, string defaultValue)
        {
            string s;
            if (_parsed.TryGetValue(key, out s))
            {
                return s;
            }
            else
            {
                return defaultValue;
            }
        }

        private void SetValue(string key, string value)
        {
            if (value == null && _parsed.ContainsKey(key))
            {
                _parsed.Remove(key);
            }
            else
            {
                _parsed[key] = value;
            }            
        }

        #endregion

        #region Properties

        public string UserName
        {
            get
            {
               return GetValue("uid", null);
            }
            set
            {
                SetValue("uid", value);
            }
        }

        public string UserIdentifier
        {
            get
            {
                return GetValue("u", null);
            }
            set
            {
                SetValue("u", value);
            }
        }

        public DateTimeOffset ValidUntil
        {
            get
            {
                long val = long.Parse(GetValue(nameof(ValidUntil).ToLowerInvariant(), "0"));

                return ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(val);
            }
            set
            {
                SetValue(nameof(ValidUntil).ToLowerInvariant(), value.Subtract(ModAuthPubTktAlgorithm.UNIX_EPOCH).TotalSeconds.ToString("0"));
            }
        }

        /// <summary>
        /// Gets the grace period date/time, or null if no "graceperiod" field was provided in the ticket.
        /// </summary>
        public DateTimeOffset? GracePeriod
        {
            get
            {
                long val = long.Parse(GetValue(nameof(GracePeriod).ToLowerInvariant(), "0"));

                if (val == 0)
                {
                    return null;
                }

                return ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(val);
            }
            set
            {
                if (value.HasValue)
                {
                    SetValue(nameof(GracePeriod).ToLowerInvariant(), value.Value.Subtract(ModAuthPubTktAlgorithm.UNIX_EPOCH).TotalSeconds.ToString("0"));
                }
                else
                {
                    SetValue(nameof(GracePeriod).ToLowerInvariant(), null);
                }
            }
        }

        /// <summary>
        /// Gets the date/time the ticket was issued in UTC, or null if no "issued" field was provided in the ticket.
        /// </summary>
        public DateTimeOffset? Issued
        {
            get
            {
                long val = long.Parse(GetValue(nameof(Issued).ToLowerInvariant(), "0"));

                if (val == 0)
                {
                    return null;
                }

                return ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(val);
            }
            set
            {
                if (value.HasValue)
                {
                    SetValue(nameof(Issued).ToLowerInvariant(), value.Value.Subtract(ModAuthPubTktAlgorithm.UNIX_EPOCH).TotalSeconds.ToString("0"));
                }
                else
                {
                    SetValue(nameof(Issued).ToLowerInvariant(), null);
                }
            }
        }

        public string ClientIP
        {
            get
            {
                return GetValue("cip", null);
            }
            set
            {
                SetValue("cip", value);
            }
        }

        public IEnumerable<string> Tokens
        {
            get
            {
                var val = GetValue(nameof(Tokens).ToLowerInvariant(), null);

                if (val == null)
                {
                    return null;
                }

                return val.Split(',');
            }
            set
            {
                if (value == null)
                {
                    SetValue(nameof(Tokens).ToLowerInvariant(), null);
                }
                else
                {
                    SetValue(nameof(Tokens).ToLowerInvariant(), string.Join(",", value));
                }
            }
        }

        public string UserData
        {
            get
            {
                return GetValue("udata", null);
            }
            set
            {
                SetValue("udata", null);
            }
        }

        public string BasicAuth
        {
            get
            {
                return GetValue("bauth", null);
            }
            set
            {
                SetValue("bauth", null);
            }
        }

        /// <summary>
        /// Gets the signature (digest) associated with the ticket data.
        /// </summary>
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
            set
            {
                SetValue("sig", Convert.ToBase64String(value));
            }
        }

        /// <summary>
        /// Gets the portion of the ticket string that makes up the signed data.
        /// </summary>
        public string DataString
        {
            get
            {
                var idx = _value.IndexOf(";sig=");

                return _value.Substring(0, idx);
            }
        }

        public IDictionary<string, string> GetElements()
        {
            return new Dictionary<string, string>(_parsed);
        }

        public void AddValue(string key, string value)
        {
            this._parsed.Add(key, value);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _parsed.TryGetValue(key, out value);
        }

        public System.Security.Claims.ClaimsIdentity CreateIdentity(string uidClaim, string tokensClaim = ClaimTypes.Role, string udataClaim = null, string authenticationMethod = "mod_auth_pubtkt")
        {
            string atype;
            var claims = new List<Claim>();
            
            claims.Add(new Claim(ClaimTypes.Name, this.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, this.UserIdentifier ?? this.UserName));

            if (uidClaim != ClaimTypes.Name)
            {
                claims.Add(new Claim(uidClaim, this.UserName));
            }

            if (this.Tokens != null)
            {
                claims.AddRange(this.Tokens.Select(s => new Claim(tokensClaim, s)));
            }

            if (this.UserData != null && udataClaim != null)
            {
                claims.Add(new Claim(udataClaim, this.UserData));
            }

            atype = claims.Where(x => x.Type == ClaimTypes.AuthenticationMethod).Select(s => s.Value).FirstOrDefault();
            if (atype == null)
            {
                atype = this.GetValue("atype", authenticationMethod);
            }

            return new System.Security.Claims.ClaimsIdentity(claims, authenticationMethod);
        }

        #endregion
    }
}