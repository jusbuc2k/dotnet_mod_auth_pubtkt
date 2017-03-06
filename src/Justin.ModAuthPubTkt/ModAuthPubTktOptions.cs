using System;

namespace Justin.ModAuthPubTkt
{
    public class ModAuthPubTktOptions
    {
        public string KeyFile { get; set; }

        public string KeyPassword { get;set; }

        public string CookieName { get; set; } = "auth_pubtkt";

        public string UserNameClaim { get; set; } = System.Security.Claims.ClaimTypes.Name;

        public string UserIdentifierClaim { get; set; } = System.Security.Claims.ClaimTypes.NameIdentifier;

        public string TokensClaim { get; set; } = System.Security.Claims.ClaimTypes.Role;

        public string UDataClaim { get; set; }
    }
}
