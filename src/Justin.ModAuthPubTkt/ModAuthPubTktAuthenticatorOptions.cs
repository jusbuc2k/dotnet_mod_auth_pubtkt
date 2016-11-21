using System;

namespace Justin.ModAuthPubTkt
{
    public class ModAuthPubTktAuthenticatorOptions : ModAuthPubTktOptions
    {
        public int CacheDuration { get; set; }

        public string LoginUrl { get; set; }
    }

}