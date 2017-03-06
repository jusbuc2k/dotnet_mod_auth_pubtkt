using Justin.AspNetCore.ModAuthPubTkt;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseModAuthPubTkt(this IApplicationBuilder builder, ModAuthPubTktAuthenticationOptions options)
        {
            return builder.UseMiddleware<ModAuthPubTktMiddleware>(Options.Create(options));
        }
    }
}
