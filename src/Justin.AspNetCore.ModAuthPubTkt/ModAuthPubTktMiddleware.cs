using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Linq;
using System;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktMiddleware
    {
        private readonly RequestDelegate _next;
        
        private readonly ILogger _logger;

        private readonly ModAuthPubTktOptions _options;

        private readonly ModAuthPubTktAlgorithm _signer;

        public ModAuthPubTktMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<ModAuthPubTktOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ModAuthPubTktMiddleware>();
            _options = options.Value;
            _signer = new ModAuthPubTktAlgorithm(_options.KeyFile, _options.KeyPassword);
        }

        public async Task Invoke(HttpContext context)
        {
            string cookieValue;
            string tmp;
            bool requireAuth = false;
            string redirectUrl = "/Account/Login";
            IDictionary<string, string> elements;

            if (context.Request.Cookies.TryGetValue(_options.CookieName, out  cookieValue))
            {
                _logger.LogDebug("ModAuthPubTkt: Auth cookie exists");
                if (_signer.Verify(cookieValue))
                {
                    _logger.LogDebug("ModAuthPubTkt: Auth cookie signature is valid.");
                    elements = Utils.ParseTicket(cookieValue);

                    if (elements.TryGetValue("validuntil", out tmp))
                    {
                        if (ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(long.Parse(tmp)) < DateTimeOffset.UtcNow)
                        {
                            _logger.LogDebug("ModAuthPubTkt: Auth cookie is valid and not expired.");

                            var identity = new System.Security.Claims.ClaimsIdentity();

                            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "ModAuthPubTkt"));
                            identity.AddClaim(new Claim(ClaimTypes.Name, elements["uid"]));

                            if (_options.UidClaim != ClaimTypes.Name)
                            {
                                identity.AddClaim(new Claim(_options.UidClaim, elements["uid"]));
                            }

                            if (elements["tokens"] != null)
                            {
                                identity.AddClaims(elements["tokens"].Split(',').Select(s => new Claim(_options.TokensClaim, s)));
                            }
                            
                            if (elements["udata"] != null)
                            {
                                identity.AddClaim(new Claim(_options.UDataClaim, elements["udata"]));
                            } 

                            context.User = new System.Security.Claims.ClaimsPrincipal(identity);
                        }
                        else
                        {
                             _logger.LogError("ModAuthPubTkt: Auth cookie is expired.");
                        }
                    }
                    else
                    {
                        _logger.LogError("ModAuthPubTkt: Auth cookie does not have a validuntil value.");
                    }
                }
            }

            if (requireAuth && (context.User == null || !context.User.Identity.IsAuthenticated))
            {
                _logger.LogError("ModAuthPubTkt: Redirecting to login page.");
                context.Response.Redirect(redirectUrl, false);
                return;
            }
            
            await _next.Invoke(context);
        }
    }
}