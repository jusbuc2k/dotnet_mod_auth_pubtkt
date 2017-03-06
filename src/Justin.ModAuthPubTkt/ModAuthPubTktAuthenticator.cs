using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Justin.ModAuthPubTkt.Abstractions;

namespace Justin.ModAuthPubTkt
{
    public class ModAuthPubTktAuthenticator : IDisposable
    {
        private readonly IPlatformAbstraction _platform;
        private readonly ModAuthPubTktSignInOptions _options;
        private readonly Justin.ModAuthPubTkt.ModAuthPubTktAlgorithm _signer;

        public ModAuthPubTktAuthenticator(ModAuthPubTktSignInOptions options, IPlatformAbstraction platform)
        {
            _options = options;
            _platform = platform;
            _signer = new ModAuthPubTktAlgorithm(options.KeyFile, options.KeyPassword);
        }

        public bool Authenticate(bool automaticChallenge = true)
        {
            Justin.ModAuthPubTkt.ModAuthPubTkt ticket = null;
            string cookieValue;        

            if (!_platform.TryGetCookie(_options.CookieName, out cookieValue))
            {
                return false;
            }

            if (cookieValue == null)
            {
                return false;
            }
            
            if (_platform.TryGetCacheEntry<Justin.ModAuthPubTkt.ModAuthPubTkt>(cookieValue, out ticket))
            {
                // ticket was cached
            }
            else if (_signer.Verify(cookieValue))
            {
                ticket = new Justin.ModAuthPubTkt.ModAuthPubTkt(cookieValue);
            }
            else
            {
                return false;
            }

            if (ticket.ValidUntil < DateTimeOffset.UtcNow)
            {
                _platform.SetCacheEntry<object>(cookieValue, null, DateTimeOffset.MinValue);
                return false;
            }

            if (ticket.GracePeriod.HasValue && ticket.GracePeriod.Value < DateTimeOffset.UtcNow)
            {
                if (automaticChallenge)
                {
                    _platform.SetResponseStatus(401, "Unauthorized");
                }
            }

            _platform.SetCacheEntry(cookieValue, ticket, DateTimeOffset.Now.AddSeconds(_options.CacheDuration));

            _platform.SetPrincipal(new System.Security.Claims.ClaimsPrincipal(ticket.CreateIdentity(_options.UserNameClaim,
                tokensClaim: _options.TokensClaim,
                udataClaim: _options.UDataClaim    
            )));

            return true;
        }

        public void Dispose()
        {
            _signer.Dispose();
        }
    }
}
