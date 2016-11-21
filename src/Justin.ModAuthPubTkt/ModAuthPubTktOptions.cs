using System;

namespace Justin.ModAuthPubTkt
{
    public class ModAuthPubTktOptions
    {
        public ModAuthPubTktOptions()
        {
            this.CookieName = "auth_pubtkt";
            this.UidClaim = System.Security.Claims.ClaimTypes.Name;
            this.TokensClaim = System.Security.Claims.ClaimTypes.Role;
        }

        public string KeyFile { get; set; }

        public string KeyPassword { get;set; }

        public string CookieName { get; set; }

        public string UidClaim { get;set; }

        public string TokensClaim { get; set; }
        
        public string UDataClaim { get; set; }

    }
}