using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class AspNetCorePlatform : Justin.ModAuthPubTkt.Abstractions.IPlatformAbstraction
    {
        private readonly Microsoft.AspNetCore.Http.HttpContext _context;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        public AspNetCorePlatform(Microsoft.AspNetCore.Http.HttpContext context, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public System.Security.Claims.ClaimsPrincipal User { get; set; }

        public void Redirect(string url)
        {
            throw new NotImplementedException();
        }

        public void SetCacheEntry<T>(string cacheKey, T cacheEntry, DateTimeOffset expires)
        {
            var cacheItem = _cache.CreateEntry(cacheKey);
            cacheItem.Value = cacheEntry;
            cacheItem.AbsoluteExpiration = expires;
        }

        public void SetCookie(string cookieName, string cookieValue, string domain, string path, DateTimeOffset? expires, bool secure)
        {
            _context.Response.Cookies.Append(cookieName, cookieValue, new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Domain = domain,
                Path = path,
                Expires = expires,
                HttpOnly = true,
                Secure = secure
            });
        }

        public void SetPrincipal(ClaimsPrincipal user)
        {
            this.User = user;
            //_context.User = user;
        }

        public bool TryGetCacheEntry<T>(string cacheKey, out T cacheEntry)
        {
            cacheEntry = default(T);
            object cacheItem;

            if(_cache.TryGetValue(cacheKey, out cacheItem))
            {
                if (cacheItem is T)
                {
                    cacheEntry = (T)cacheItem;
                    return true;
                }
                return false;
            }

            return false;
        }

        public bool TryGetCookie(string cookieName, out string cookieValue)
        {
            return _context.Request.Cookies.TryGetValue(cookieName, out cookieValue);
        }
    }
}
