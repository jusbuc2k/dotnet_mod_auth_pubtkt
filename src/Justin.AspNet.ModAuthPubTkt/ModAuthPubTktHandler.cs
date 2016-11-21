﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Justin.AspNet.ModAuthPubTkt
{
    public class ModAuthPubTktHttpModule : System.Web.IHttpModule
    {
        private readonly Justin.ModAuthPubTkt.ModAuthPubTktAuthenticatorOptions _options;

        public ModAuthPubTktHttpModule()
        {
            Justin.ModAuthPubTkt.ModAuthPubTktAuthenticatorOptions authenticatorOptions = null;
            if (System.Web.Mvc.DependencyResolver.Current != null)
            {
                authenticatorOptions = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(Justin.ModAuthPubTkt.ModAuthPubTktAuthenticatorOptions)) as Justin.ModAuthPubTkt.ModAuthPubTktAuthenticatorOptions;
            }

            if (authenticatorOptions == null)
            {
                authenticatorOptions = new Justin.ModAuthPubTkt.ModAuthPubTktAuthenticatorOptions();
                authenticatorOptions.CookieName = System.Configuration.ConfigurationManager.AppSettings.GetValue("ModAuthPubTkt.CookieName", authenticatorOptions.CookieName);
                authenticatorOptions.KeyFile = System.Configuration.ConfigurationManager.AppSettings.GetValue("ModAuthPubTkt.KeyFile");
                authenticatorOptions.KeyPassword = System.Configuration.ConfigurationManager.AppSettings.GetValue("ModAuthPubTkt.KeyPassword");
                authenticatorOptions.UidClaim = System.Configuration.ConfigurationManager.AppSettings.GetValue("ModAuthPubTkt.UidClaim", authenticatorOptions.UidClaim);
                authenticatorOptions.UDataClaim = System.Configuration.ConfigurationManager.AppSettings.GetValue("ModAuthPubTkt.UDataClaim");
                authenticatorOptions.TokensClaim = System.Configuration.ConfigurationManager.AppSettings.GetValue("ModAuthPubTkt.TokensClaim", authenticatorOptions.TokensClaim);
                authenticatorOptions.CacheDuration = int.Parse(System.Configuration.ConfigurationManager.AppSettings.GetValue("ModAuthPubTkt.CacheDuration", "60"));
            }

            this.Init();
        }

        public ModAuthPubTktHttpModule(Justin.ModAuthPubTkt.ModAuthPubTktAuthenticatorOptions options)
        {
            _options = options;
            this.Init();
        }

        protected void Init()
        {
            
        }

        void IHttpModule.Dispose()
        {
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.PostAcquireRequestState += (sender, e) =>
            {
                var platform = new SystemWebPlatform(new HttpContextWrapper(context.Context), System.Runtime.Caching.MemoryCache.Default);

                var s = new Justin.ModAuthPubTkt.ModAuthPubTktAuthenticator(_options, platform);

                s.Authenticate();
            };

            context.EndRequest += (sender, e) =>
            {
                if (context.Response.StatusCode == 401)
                {
                    context.Response.Status = "302 Found";
                    context.Response.RedirectLocation = _options.LoginUrl;
                }
            };
        }

    }
}
