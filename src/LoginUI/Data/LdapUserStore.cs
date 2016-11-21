using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Data
{
    public class LdapUserStoreOptions
    {
        public string Hostname { get; set; }
    }

    public class LdapUserStore : WebApplication.Services.IUserPasswordAuthenticator, IDisposable
    {
        private readonly LdapUserStoreOptions _options;

        public LdapUserStore(IOptions<LdapUserStoreOptions> options)
        {
            _options = options.Value;
        }

        public void Dispose()
        {
        }

        public bool ValidatePassword(string username, string password)
        {

#if NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 
            using (var conn = new System.DirectoryServices.Protocols.LdapConnection(_options.Hostname))
            {
                try
                {
                    conn.Bind(new System.Net.NetworkCredential(username, password));

                    return true;
                }
                catch (System.DirectoryServices.Protocols.LdapException ex)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
#else
            throw new NotSupportedException("LDAP is currently not supported on NET Standard");
#endif
        }

        public string GetNormalizedUsername(string username)
        {
            return username;
        }

        public string GetDisplayName(string username)
        {
            return username;
        }

    }
}
