using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public interface IUserStore<TUser, TKey>
    {
        TUser FindByUserName(string userName);

        TUser FindByUserID(TKey userID);

        IEnumerable<TUser> GetList();

        IEnumerable<TUser> GetFilteredList(string nameFilter);
    }

    public interface IUserTwoFactorStore<TUser>
    {
        string GetTwoFactorSecret(TUser user);

        bool IsTwoFactorEnabled(TUser user);
    }

    public interface IUserPasswordStore<TUser>
    {
        string GetPasswordHash(TUser user);

        void SetPasswordHash(TUser user, string passwordHash);
    }

    public class ApplicationPassword
    {
        public string ApplicationName { get; set; }

        public string Description { get; set; }

        public string PasswordHash { get; set; }

        public DateTimeOffset LastUsed { get; set; }
    }

    public interface IUserApplicationPasswordStore<TUser>
    {
        IEnumerable<ApplicationPassword> GetApplicationPasswords(TUser user);

        void RemoveApplicationPassword(TUser user, ApplicationPassword password);

        void AddApplicationPassword(TUser user, ApplicationPassword password);
    }

}
