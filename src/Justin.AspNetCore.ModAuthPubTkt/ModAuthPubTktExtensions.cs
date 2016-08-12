using Justin.AspNetCore.ModAuthPubTkt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ModAuthPubTktExtensions
{
    public static void AddModAuthPubTktTokenService(this IServiceCollection services, IConfigurationSection configSection)
    {
        services.Configure<ModAuthPubTktOptions>(x => {
            var section = configSection;
            x.KeyFile = section["keyFile"];
            x.KeyPassword = section["keyPassword"];
            x.CookieName = section["cookieName"];
            x.UidClaim = section["uidClaim"];
            x.TokensClaim = section["tokensClaim"];
            x.UDataClaim = section["udataClaim"];
            x.ClientIpSource = section["clientIpSource"];
            x.ValidMinutes = string.IsNullOrEmpty(section["validMinutes"]) ? 30 * 60 : int.Parse(section["validMinutes"]);
            x.GraceMinutes = string.IsNullOrEmpty(section["graceMinutes"]) ? 5 * 60 : int.Parse(section["graceMinutes"]);
            x.FakeBasicKey = section["FakeBasicKey"];
            x.FakeBasicWithRealPassword = string.IsNullOrEmpty(section["fakeBasicWithRealPassword"]) ? false : bool.Parse(section["fakeBasicWithRealPassword"]);
            x.CookieDomain = section["cookieDomain"];
            x.CookieSecure = string.IsNullOrEmpty(section["cookieSecure"]) ? true : bool.Parse(section["cookieSecure"]);
        });

        services.AddTransient<IAuthTokenService, ModAuthPubTktAuthTokenService>();
    }

    public static IApplicationBuilder UseModAuthPubTkt(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ModAuthPubTktMiddleware>();
    }
}

