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
        private readonly ModAuthPubTktAuthenticatorOptions _options;
        private readonly Justin.ModAuthPubTkt.ModAuthPubTktAlgorithm _signer;

        public ModAuthPubTktAuthenticator(ModAuthPubTktAuthenticatorOptions options, IPlatformAbstraction platform)
        {
            _options = options;
            _platform = platform;
            _signer = new ModAuthPubTktAlgorithm(options.KeyFile, options.KeyPassword);
        }

        public bool Authenticate()
        {
            Justin.ModAuthPubTkt.ModAuthPubTkt? ticket = null;
            string cookieValue;        

            if (!_platform.TryGetCookie(_options.CookieName, out cookieValue))
            {
                return false;
            }

            if (cookieValue == null)
            {
                return false;
            }
            
            if (_platform.TryGetCacheEntry<Justin.ModAuthPubTkt.ModAuthPubTkt?>(cookieValue, out ticket))
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

            if (ticket.Value.ValidUntil < DateTimeOffset.UtcNow)
            {
                _platform.SetCacheEntry<object>(cookieValue, null, DateTimeOffset.MinValue);
                return false;
            }

            if (ticket.Value.GracePeriod.HasValue && ticket.Value.GracePeriod.Value < DateTimeOffset.UtcNow)
            {
                _platform.Redirect(_options.LoginUrl);
            }

            _platform.SetCacheEntry(cookieValue, ticket, DateTimeOffset.Now.AddSeconds(_options.CacheDuration));

            _platform.SetPrincipal(new System.Security.Claims.ClaimsPrincipal(ticket.Value.CreateIdentity(_options.UidClaim,
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
