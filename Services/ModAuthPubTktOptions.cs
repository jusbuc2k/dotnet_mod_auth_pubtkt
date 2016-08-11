using System;
using Microsoft.Extensions.Options;

namespace Webapplication1
{

    public class ModAuthPubTktOptions
    {
        public string KeyFile { get; set; }

        public string KeyPassword { get;set; }

        public string CookieName { get; set; }

        public string UidClaim {get;set;}

        public string TokensClaim { get; set; }
        
        public string UDataClaim { get; set; }

        public string ClientIpSource { get; set; }

        public int ValidSeconds { get; set; }

        public int GraceSeconds { get; set; }

        public string FakeBasicKey { get; set; }

        public bool FakeBasicWithRealPassword { get; set; }

        public bool CookieSecure { get; set; }

        public string CookieDomain { get; set; }

    }

}