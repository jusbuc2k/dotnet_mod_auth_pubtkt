using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using Justin.AspNetCore.ModAuthPubTkt;
using Justin.ModAuthPubTkt;

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
            var auth_mode = this.Configuration.GetValue<string>("Auth_Mode");

            services.AddOptions();

            if (auth_mode == "LDAP")
            {
                services.Configure<LdapUserStoreOptions>(this.Configuration.GetSection("Auth_Ldap"));
                services.AddTransient<WebApplication.Services.IUserPasswordAuthenticator, WebApplication.Data.LdapUserStore>();
            }
            else if (auth_mode == "MySQL")
            {
                services.Configure<MySqlUserStoreOptions>(this.Configuration.GetSection("Auth_MySQL"));
                services.AddTransient<WebApplication.Services.IUserPasswordAuthenticator, WebApplication.Data.MySqlUserStore>();
            }
            else
            {
                throw new Exception("Unsupported authentication mode.");
            }    

            services.Configure<ModAuthPubTktTokenServiceOptions>(this.Configuration.GetSection("mod_auth_pubtkt"));
            
            services.AddModAuthPubTktTokenService();

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
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseModAuthPubTkt();

            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
