using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public interface IAuthTokenService
    {
        Task<bool> Validate(Microsoft.AspNetCore.Http.HttpRequest request);

        Task SignIn(Microsoft.AspNetCore.Http.HttpResponse response, System.Security.Claims.ClaimsPrincipal user, string userPassword);

        Task SignOut(Microsoft.AspNetCore.Http.HttpResponse response, System.Security.Claims.ClaimsPrincipal user);
    }
}
