using Justin.AspNetCore.ModAuthPubTkt;
using Justin.ModAuthPubTkt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddModAuthPubTkt<TUser>(this IServiceCollection collection)
            where TUser: class
        {
            collection.AddScoped<Microsoft.AspNetCore.Identity.SignInManager<TUser>, ModAuthPubTktSignInManager<TUser>>();
        }
    }
}
