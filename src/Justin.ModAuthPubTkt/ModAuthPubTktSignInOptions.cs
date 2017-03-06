using System;

namespace Justin.ModAuthPubTkt
{
    public class ModAuthPubTktSignInOptions : ModAuthPubTktOptions
    {
        public int CacheDuration { get; set; } = 5;

        public string LoginUrl { get; set; }

        public string ClientIpSource { get; set; }

        public int ValidMinutes { get; set; } = 15;

        public int GraceMinutes { get; set; } = 5;

        public string FakeBasicKey { get; set; }

        public bool FakeBasicWithRealPassword { get; set; }

        public bool CookieSecure { get; set; }

        public string CookieDomain { get; set; }

        public bool SetIssued { get; set; }

        public string AdditionalClaims { get; set; }
    }

}