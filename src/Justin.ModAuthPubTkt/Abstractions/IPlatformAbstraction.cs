using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Justin.ModAuthPubTkt.Abstractions
{
    public interface IPlatformAbstraction
    {
        bool TryGetCookie(string cookieName, out string cookieValue);

        void SetCookie(string cookieName, string cookieValue, string domain, string path, DateTimeOffset? expires, bool secure);

        bool TryGetCacheEntry<T>(string cacheKey, out T cacheEntry);

        void SetCacheEntry<T>(string cacheKey, T cacheEntry, DateTimeOffset expires);

        void Redirect(string url);

        void SetPrincipal(System.Security.Claims.ClaimsPrincipal user);
    }
}
