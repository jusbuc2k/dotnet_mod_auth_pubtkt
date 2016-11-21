using Justin.AspNetCore.ModAuthPubTkt;
using Justin.ModAuthPubTkt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ModAuthPubTktExtensions
{
    public static void AddModAuthPubTktTokenService(this IServiceCollection services)
    {
        services.AddTransient<IAuthTokenService, ModAuthPubTktAuthTokenService>();
    }

    public static IApplicationBuilder UseModAuthPubTkt(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ModAuthPubTktMiddleware>();
    }
}

