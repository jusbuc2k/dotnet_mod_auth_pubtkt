using System;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktAuthenticationOptions : Microsoft.AspNetCore.Builder.AuthenticationOptions
    {
        public Microsoft.Extensions.Caching.Memory.IMemoryCache Cache { get; set; }

        public Justin.ModAuthPubTkt.ModAuthPubTktAuthenticatorOptions MiddlewareOptions { get; set; }
    }
}