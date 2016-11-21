using System;

namespace Justin.ModAuthPubTkt
{
    public class ModAuthPubTktTokenServiceOptions: ModAuthPubTktOptions
    {

        public string ClientIpSource { get; set; }

        public int ValidMinutes { get; set; }

        public int GraceMinutes { get; set; }

        public string FakeBasicKey { get; set; }

        public bool FakeBasicWithRealPassword { get; set; }

        public bool CookieSecure { get; set; }

        public string CookieDomain { get; set; }

        public bool SetIssued { get; set; }

    }
}