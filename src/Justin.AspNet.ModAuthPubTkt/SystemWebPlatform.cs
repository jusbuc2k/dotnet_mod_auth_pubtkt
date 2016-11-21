using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Justin.AspNet.ModAuthPubTkt
{
    public class SystemWebPlatform : Justin.ModAuthPubTkt.Abstractions.IPlatformAbstraction
    {
        private readonly System.Web.HttpContextBase _context;
        private readonly System.Runtime.Caching.ObjectCache _cache;

        public SystemWebPlatform(System.Web.HttpContextBase context, System.Runtime.Caching.MemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public void Redirect(string url)
        {
            _context.Response.Redirect(url);
        }

        public void SetCacheEntry<T>(string cacheKey, T cacheEntry, DateTimeOffset expires)
        {
            throw new NotImplementedException();
        }

        public void SetCookie(string cookieName, string cookieValue, string domain, string path, DateTimeOffset? expires, bool secure)
        {
            throw new NotImplementedException();
        }

        public void SetPrincipal(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCacheEntry<T>(string cacheKey, out T cacheEntry)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCookie(string cookieName, out string cookieValue)
        {
            throw new NotImplementedException();
        }
    }
}
