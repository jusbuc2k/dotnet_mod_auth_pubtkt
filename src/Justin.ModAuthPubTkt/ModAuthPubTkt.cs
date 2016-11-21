using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Justin.ModAuthPubTkt
{
    public struct ModAuthPubTkt
    {
        #region Privates

        private readonly string _value;
        private readonly IDictionary<string, string> _parsed;

        #endregion

        #region Constructor

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
            if (_parsed.TryGetValue("uid", out s))
            {
                return s;
            }
            else
            {
                return defaultValue;
            }
        }

        #endregion

        #region Properties

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

        /// <summary>
        /// Gets the grace period date/time, or null if no "graceperiod" field was provided in the ticket.
        /// </summary>
        public DateTimeOffset? GracePeriod
        {
            get
            {
                long val = long.Parse(GetValue("graceperiod", "0"));

                if (val == 0)
                {
                    return null;
                }

                return ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(val);
            }
        }

        /// <summary>
        /// Gets the date/time the ticket was issued in UTC, or null if no "issued" field was provided in the ticket.
        /// </summary>
        public DateTimeOffset? Issued
        {
            get
            {
                long val = long.Parse(GetValue("issued", "0"));

                if (val == 0)
                {
                    return null;
                }

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

        public System.Security.Claims.ClaimsIdentity CreateIdentity(string uidClaim, string tokensClaim = ClaimTypes.Role, string udataClaim = null, string authenticationMethod = "mod_auth_pubtkt")
        {
            var identity = new System.Security.Claims.ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            identity.AddClaim(new Claim(ClaimTypes.Name, this.UserID));

            if (uidClaim != ClaimTypes.Name)
            {
                identity.AddClaim(new Claim(uidClaim, this.UserID));
            }

            if (this.Tokens != null)
            {
                identity.AddClaims(this.Tokens.Select(s => new Claim(tokensClaim, s)));
            }

            if (this.UserData != null && udataClaim != null)
            {
                identity.AddClaim(new Claim(udataClaim, this.UserData));
            }

            return identity;
        }

        #endregion
    }
}