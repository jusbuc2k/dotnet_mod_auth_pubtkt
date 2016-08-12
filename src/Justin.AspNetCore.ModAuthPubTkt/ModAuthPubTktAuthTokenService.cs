using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktAuthTokenService : IAuthTokenService, IDisposable
    {
        private static string EncryptFakeBasicPassword(string key, string password)
        {
            throw new NotImplementedException(); 
        }

        protected readonly ModAuthPubTktAlgorithm _signer;

        protected readonly ModAuthPubTktOptions _options;

        public ModAuthPubTktAuthTokenService(IOptions<ModAuthPubTktOptions> options)
        {            
            _options = options.Value;
            _signer = new ModAuthPubTktAlgorithm(options.Value.KeyFile, options.Value.KeyPassword);
            this.CookieName = options.Value.CookieName;
        }

        public string CookieName { get; set; }

        public Task<bool> Validate(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            string cookieValue;

            if (!request.Cookies.TryGetValue(this.CookieName, out cookieValue))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(_signer.Verify(cookieValue));
        }

        public Task SignIn(Microsoft.AspNetCore.Http.HttpResponse response, System.Security.Claims.ClaimsPrincipal user, string userPassword)
        {
            string ticket;
            string uid;
            string cip = null;
            IEnumerable<string> tokens = null;
            string udata = null;
            DateTimeOffset expires = DateTimeOffset.UtcNow.AddMinutes(_options.ValidMinutes);

            if (string.IsNullOrEmpty(_options.UidClaim))
            {
                uid = user.Identity.Name;
            }
            else
            {
                uid = user.Claims.Where(x => x.Type == _options.UidClaim).Select(s => s.Value).First();
            }

            if (!string.IsNullOrEmpty(_options.ClientIpSource))
            {
                if (_options.ClientIpSource == "REMOTE_ADDR")
                {
                    cip = response.HttpContext.Connection.RemoteIpAddress.ToString();
                }
            }

            if (!string.IsNullOrEmpty(_options.TokensClaim))
            {
                tokens = user.Claims.Where(x => x.Type == _options.TokensClaim).Select(s => s.Value);
            }

            if (!string.IsNullOrEmpty(_options.UDataClaim))
            {
                udata = user.Claims.Where(x => x.Type == _options.UDataClaim).Select(s => s.Value).FirstOrDefault();
            }

            ticket = _signer.Sign(uid, expires, 
                gracePeriod: TimeSpan.FromMinutes(_options.GraceMinutes),
                cip: cip,
                tokens: tokens,
                udata: udata,
                bauth: _options.FakeBasicWithRealPassword ? EncryptFakeBasicPassword(_options.FakeBasicKey, userPassword) : null
            );

            response.Cookies.Append(this.CookieName, ticket, new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Path = "/",
                HttpOnly = true,
                Secure = _options.CookieSecure,
                Domain = _options.CookieDomain
            });

            return Task.FromResult<object>(null);
        }

        public Task SignOut(Microsoft.AspNetCore.Http.HttpResponse response, System.Security.Claims.ClaimsPrincipal user)
        {
            response.Cookies.Delete(this.CookieName);
            return Task.FromResult<object>(null);
        }
        public void Dispose()
        {
            _signer.Dispose();
        }
    }
}
