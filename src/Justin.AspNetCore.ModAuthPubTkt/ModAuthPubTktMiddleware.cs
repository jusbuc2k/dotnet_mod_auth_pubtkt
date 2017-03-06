using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authentication;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktMiddleware : Microsoft.AspNetCore.Authentication.AuthenticationMiddleware<ModAuthPubTktAuthenticationOptions>
    {
        private readonly ILogger _logger;

        public ModAuthPubTktMiddleware(RequestDelegate next, 
            IOptions<ModAuthPubTktAuthenticationOptions> options,
            ILoggerFactory loggerFactory, 
            System.Text.Encodings.Web.UrlEncoder encoder,
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache) : base(next, options, loggerFactory, encoder)
        {            
            _logger = loggerFactory.CreateLogger<ModAuthPubTktMiddleware>();
        }
                
        protected override AuthenticationHandler<ModAuthPubTktAuthenticationOptions> CreateHandler()
        {
            return new ModAuthPubTktAuthenticationHandler();
        }
    }
}