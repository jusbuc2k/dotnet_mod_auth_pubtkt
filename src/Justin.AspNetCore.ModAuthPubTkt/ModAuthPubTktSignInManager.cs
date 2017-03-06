using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktSignInManager<TUser> : Microsoft.AspNetCore.Identity.SignInManager<TUser>
        where TUser: class
    {
        private readonly ModAuthPubTktAuthenticationOptions _pubTktOptions;
        private readonly HttpContext _context;

        public ModAuthPubTktSignInManager(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger, IOptions<ModAuthPubTktAuthenticationOptions> pubTktOptionsAccessor) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            _context = contextAccessor.HttpContext;
            _pubTktOptions = pubTktOptionsAccessor.Value;
        }

        public override async Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            var userPrincipal = await base.CreateUserPrincipalAsync(user);

            if (authenticationMethod != null)
            {
                userPrincipal.Identities.First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            }

            await _context.Authentication.SignInAsync(ModAuthPubTktAuthenticationOptions.DefaultAuthenticationScheme, userPrincipal, authenticationProperties);
        }

        public override async Task SignOutAsync()
        {
            await base.SignOutAsync();
            await _context.Authentication.SignOutAsync(ModAuthPubTktAuthenticationOptions.DefaultAuthenticationScheme);
        }
    }
}
