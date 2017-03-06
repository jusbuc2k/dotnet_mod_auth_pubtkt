using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Justin.ModAuthPubTkt;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktAuthenticationHandler : Microsoft.AspNetCore.Authentication.AuthenticationHandler<ModAuthPubTktAuthenticationOptions>
    {
        private static readonly string[] ForwardingHeaders = new string[] { "X-FORWARDED-FOR", "X-PROXYUSER-IP", "X-REAL-IP" };
        private static string EncryptFakeBasicPassword(string key, string password)
        {
            throw new NotImplementedException();
        }

        public ModAuthPubTktAuthenticationHandler() : base()
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var platform = new AspNetCorePlatform(this.Context, this.Options.Cache);

            using (var authenticator = new Justin.ModAuthPubTkt.ModAuthPubTktAuthenticator(this.Options.MiddlewareOptions, platform))
            {
                if (authenticator.Authenticate(this.Options.AutomaticChallenge))
                {
                    return Task.FromResult<AuthenticateResult>(AuthenticateResult.Success(
                        new AuthenticationTicket(platform.User,
                            new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties(),
                            ModAuthPubTktAuthenticationOptions.DefaultAuthenticationScheme)
                        )
                    );
                }
                else
                {
                    return Task.FromResult(AuthenticateResult.Skip());
                }
            }
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            ModAuthPubTktAlgorithm _signer;
            var _options = this.Options.MiddlewareOptions;
            var user = context.Principal;
            var properties = new AuthenticationProperties(context.Properties);

            using (_signer = new ModAuthPubTktAlgorithm(_options.KeyFile, _options.KeyPassword))
            {
                string cookieValue;
                DateTimeOffset expires = DateTimeOffset.UtcNow.AddMinutes(_options.ValidMinutes);
                var ticket = new Justin.ModAuthPubTkt.ModAuthPubTkt();

                ticket.AddValue("atype", user.Identity.AuthenticationType);

                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null)
                {
                    ticket.UserIdentifier = idClaim.Value;
                }

                if (string.IsNullOrEmpty(_options.UserNameClaim))
                {
                    ticket.UserName = user.Identity.Name;
                }
                else
                {
                    ticket.UserName = user.Claims.Where(x => x.Type == _options.UserNameClaim).Select(s => s.Value).First();
                }

                if (!string.IsNullOrEmpty(_options.ClientIpSource))
                {
                    Rfc7232ForwardedHttpInfo forwardInfo;
                    if (_options.ClientIpSource == "*")
                    {
                        if (this.Context.Request.Headers.TryGetRfc7232Info(out forwardInfo))
                        {
                            ticket.ClientIP = forwardInfo.For;
                        }
                        else
                        {
                            ticket.ClientIP = this.Context.Request.Headers.GetFirstValue(ForwardingHeaders);
                            if (string.IsNullOrEmpty(ticket.ClientIP))
                            {
                                ticket.ClientIP = this.Context.Connection.RemoteIpAddress.ToString();
                            }
                        }
                    }
                    else if (_options.ClientIpSource == "REMOTE_ADDR")
                    {
                        ticket.ClientIP = this.Context.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        ticket.ClientIP = this.Context.Request.Headers.GetFirstValue(_options.ClientIpSource);
                    }
                }

                if (!string.IsNullOrEmpty(_options.TokensClaim))
                {
                    ticket.Tokens = user.Claims.Where(x => x.Type == _options.TokensClaim).Select(s => s.Value);
                }
                
                if (_options.UDataClaim != null)
                {
                    ticket.UserData = user.Claims.Where(x => x.Type == _options.UDataClaim).Select(s => s.Value).FirstOrDefault();
                }

                ticket.BasicAuth = _options.FakeBasicWithRealPassword ? EncryptFakeBasicPassword(_options.FakeBasicKey, null) : null;
                ticket.GracePeriod = expires.Subtract(TimeSpan.FromMinutes(_options.GraceMinutes));
                ticket.ValidUntil = expires;

                cookieValue = _signer.Sign(ticket.GetElements());

                this.Context.Response.Cookies.Append(_options.CookieName, cookieValue, new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    Secure = _options.CookieSecure,
                    Domain = _options.CookieDomain,
                    Expires = properties.IsPersistent ? expires : (DateTimeOffset?)null
                });

                return Task.CompletedTask;
            }
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
        {
            this.Context.Response.Cookies.Delete(this.Options.MiddlewareOptions.CookieName);

            return Task.CompletedTask;
        }

    }
}
