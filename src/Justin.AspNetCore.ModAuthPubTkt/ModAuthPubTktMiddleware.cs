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

        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        public ModAuthPubTktMiddleware(RequestDelegate next, 
            ILoggerFactory loggerFactory, 
            IOptions<ModAuthPubTktOptions> options, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ModAuthPubTktMiddleware>();
            _options = options.Value;
            _signer = new ModAuthPubTktAlgorithm(_options.KeyFile, _options.KeyPassword);
            _cache = cache;
        }

        public void HandleUnauthRequest(HttpContext context)
        {
            bool requireAuth = false;
            string redirectUrl = "";
            if (requireAuth && (context.User == null || !context.User.Identity.IsAuthenticated))
            {
                _logger.LogError("ModAuthPubTkt: Redirecting to login page.");
                context.Response.Redirect(redirectUrl, false);
            }
        }

        public void SetCurrentPrincipal(HttpContext context, IDictionary<string, string> ticket)
        {
            var identity = new System.Security.Claims.ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "ModAuthPubTkt"));
            identity.AddClaim(new Claim(ClaimTypes.Name, ticket["uid"]));

            if (_options.UidClaim != ClaimTypes.Name)
            {
                identity.AddClaim(new Claim(_options.UidClaim, ticket["uid"]));
            }

            if (ticket["tokens"] != null)
            {
                identity.AddClaims(ticket["tokens"].Split(',').Select(s => new Claim(_options.TokensClaim, s)));
            }
            
            if (ticket["udata"] != null)
            {
                identity.AddClaim(new Claim(_options.UDataClaim, ticket["udata"]));
            } 

            context.User = new System.Security.Claims.ClaimsPrincipal(identity);
        }

        public async Task Invoke(HttpContext context)
        {
            string cookieValue;
            string tmp;
            IDictionary<string, string> elements = null;
            object cacheEntry;

            if (!context.Request.Cookies.TryGetValue(_options.CookieName, out  cookieValue))
            {
                HandleUnauthRequest(context);
                return;
            }

            _logger.LogDebug("ModAuthPubTkt: Auth cookie exists");
            
            if(_cache.TryGetValue(cookieValue, out cacheEntry)) 
            {
                elements = cacheEntry as IDictionary<string, string>;
            }

            if (elements == null && !_signer.Verify(cookieValue))
            {
                _logger.LogDebug("ModAuthPubTkt: Auth cookie signature is *not* valid.");
                HandleUnauthRequest(context);
                return;
            }

            elements = Utils.ParseTicket(cookieValue);

            if (!elements.TryGetValue("validuntil", out tmp))
            {
                _logger.LogDebug("ModAuthPubTkt: Auth cookie has no validuntil.");
                HandleUnauthRequest(context);
                return;
            }

            if (ModAuthPubTktAlgorithm.UNIX_EPOCH.AddSeconds(long.Parse(tmp)) < DateTimeOffset.UtcNow)
            {
                _logger.LogError("ModAuthPubTkt: Auth cookie is expired.");
                HandleUnauthRequest(context);
                return;
            }

            SetCurrentPrincipal(context, elements);

            await _next.Invoke(context);
        }
    }
}