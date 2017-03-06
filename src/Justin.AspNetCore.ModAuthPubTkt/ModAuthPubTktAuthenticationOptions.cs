using System;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktAuthenticationOptions : Microsoft.AspNetCore.Builder.AuthenticationOptions
    {
        public const string DefaultAuthenticationScheme = "mod_auth_pubtkt";

        public ModAuthPubTktAuthenticationOptions()
        {
            this.AuthenticationScheme = DefaultAuthenticationScheme;
            this.AutomaticAuthenticate = true;
            this.AutomaticChallenge = false;
        }

        public Microsoft.Extensions.Caching.Memory.IMemoryCache Cache { get; set; }

        public Justin.ModAuthPubTkt.ModAuthPubTktSignInOptions MiddlewareOptions { get; set; }

    }
}