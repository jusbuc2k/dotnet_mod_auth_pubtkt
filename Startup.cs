using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using Webapplication1;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddOptions();

            services.Configure<ModAuthPubTktOptions>(x => {
                var section = Configuration.GetSection("mod_auth_pubtkt");
                x.KeyFile = section["keyFile"];
                x.KeyPassword = section["keyPassword"];
                x.CookieName = section["cookieName"];
                x.UidClaim = section["uidClaim"];
                x.TokensClaim = section["tokensClaim"];
                x.UDataClaim = section["udataClaim"];
                x.ClientIpSource = section["clientIpSource"];
                x.ValidSeconds = string.IsNullOrEmpty(section["validSeconds"]) ? 30 * 60 : int.Parse(section["validSeconds"]);
                x.GraceSeconds = string.IsNullOrEmpty(section["graceSeconds"]) ? 5 * 60 : int.Parse(section["graceSeconds"]);
                x.FakeBasicKey = section["FakeBasicKey"];
                x.FakeBasicWithRealPassword = string.IsNullOrEmpty(section["fakeBasicWithRealPassword"]) ? false : bool.Parse(section["fakeBasicWithRealPassword"]);
                x.CookieDomain = section["cookieDomain"];
                x.CookieSecure = string.IsNullOrEmpty(section["cookieSecure"]) ? true : bool.Parse(section["cookieSecure"]);
            });

            services.AddTransient<IAuthTokenService, ModAuthPubTktAuthTokenService>();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
