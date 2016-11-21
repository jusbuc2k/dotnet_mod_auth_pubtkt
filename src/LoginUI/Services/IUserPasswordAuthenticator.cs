using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public interface IUserPasswordAuthenticator
    {
        bool ValidatePassword(string username, string password);

        string GetNormalizedUsername(string username);

        string GetDisplayName(string username);
    }
}