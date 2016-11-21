using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktAuthenticationHandler : Microsoft.AspNetCore.Authentication.AuthenticationHandler<ModAuthPubTktAuthenticationOptions>
    { 
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var platform = new AspNetCorePlatform(this.Context, this.Options.Cache);

            var middleware = new Justin.ModAuthPubTkt.ModAuthPubTktAuthenticator(this.Options.MiddlewareOptions, platform);

            return Task.FromResult<AuthenticateResult>(AuthenticateResult.Success(new AuthenticationTicket(platform.User, new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties(), "mod_auth_pubtkt")));
        }
    }
}
