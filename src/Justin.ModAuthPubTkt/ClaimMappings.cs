using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Justin.ModAuthPubTkt
{
    public static class ClaimMappings
    {
        private const string IdentityPrefix = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";

        public static string Condense(string claimType)
        {
            if (claimType.StartsWith(IdentityPrefix))
            {
                return $"ani:{claimType.Substring(IdentityPrefix.Length)}";
            }
            else
            {
                return claimType;
            }
        }

        public static string Expand(string claimType)
        {
            if (claimType == null)
            {
                throw new ArgumentNullException(nameof(claimType));
            }

            if (claimType.StartsWith("ani:"))
            {
                return $"{IdentityPrefix}:{claimType.Substring(5)}";
            }
            else
            {
                return claimType;
            }
        }
    }
}
