using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Threading;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class DbUserStore : WebApplication.Services.IUserPasswordAuthenticator, IDisposable
    {
        public DbUserStore(System.Data.Common.DbConnection connection)
        {
            this.Connection = connection;
            
            if (this.Connection.State != System.Data.ConnectionState.Open)
            {
                this.Connection.Open();
                this.OwnsConnection = true;
            }
        }

        protected System.Data.Common.DbConnection Connection { get;set; }

        protected bool OwnsConnection { get; set; }

        public void Dispose()
        {
            if (this.OwnsConnection)
            {
                this.Connection.Dispose();
            }
        }

        public virtual bool ValidatePassword(string username, string password)
        {
            throw new NotImplementedException();
        }

        public string GetNormalizedUsername(string username)
        {
            return username;
        }

        public string GetDisplayName(string username)
        {
            return username;
        }

        /*
                protected string UserTableName { get; set; }

                public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
                {
                    await this.Connection.ExecuteAsync("INSERT INTO UserRole (UserID, RoleID) SELECT @UserID, RoleID FROM [Role] WHERE Name=@Name;", new
                    {
                        user.UserID,
                        Name = roleName
                    });
                }

                public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    await this.Connection.ExecuteAsync($"INSERT INTO {this.UserTableName}(UserName,Email) VALUES(@Username,@Email,@DisplayName,@TwoFactorEnabled,@IsLockedOut,@LockoutExpires);", new
                    {
                        user.UserName,
                        user.Email,
                        user.DisplayName,
                        user.TwoFactorEnabled,
                        user.IsLockedOut,
                        user.LockoutExpires
                    });

                    return IdentityResult.Success;
                }

                public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    var result = await this.Connection.ExecuteAsync($"DELETE FROM {this.UserTableName} WHERE UserID=@UserID;", new
                    {
                        user.UserID
                    });

                    return result == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError() { Code = "DeleteFailed", Description = "Unexpected rowcount when deleting user." });
                }

                public void Dispose()
                {
                }

                public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
                {
                    var result = await this.Connection.QueryAsync<ApplicationUser>($"SELECT * FROM {this.UserTableName} WHERE UserID=@UserID;", new
                    {
                        UserID = userId
                    });

                    return result.SingleOrDefault();
                }

                public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
                {
                    var result = await this.Connection.QueryAsync<ApplicationUser>($"SELECT * FROM {this.UserTableName} WHERE UserName=@UserName;", new
                    {
                        UserName = normalizedUserName
                    });

                    return result.SingleOrDefault();
                }

                public async Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return await this.Connection.ExecuteScalarAsync<int>("SELECT SUM([BadPasswordCount]) FROM [UserPassword] WHERE UserID=@UserID AND [BadPasswordDateTime] > @After;",
                        new
                        {
                            user.UserID,
                            After = DateTimeOffset.Now.AddMinutes(-15)
                        });
                }

                public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return Task.FromResult(user.IsLockedOut);
                }

                public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return Task.FromResult(user.LockoutExpires);
                }

                public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return Task.FromResult(user.UserName);
                }

                public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return await this.Connection.ExecuteScalarAsync<string>("SELECT [Password] FROM [UserPassword] WHERE UserID=@UserID AND IsDefault = 1", new
                    {
                        user.UserID
                    });
                }

                public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    var result = await this.Connection.QueryAsync<string>("SELECT R.[Name] FROM [UserRole] UR INNER JOIN [User] U ON U.[UserID] = UR.[UserID] WHERE U.[UserID]=@UserID;", new
                    {
                        user.UserID
                    });

                    return result.ToList();
                }

                public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return Task.FromResult(user.UserID.ToString());
                }

                public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return Task.FromResult(user.UserName);
                }

                public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
                {
                    return await this.Connection.QueryAsync<ApplicationUser>("SELECT U.* FROM [UserRole] UR INNER JOIN [User] U ON U.[UserID] = UR.[UserID] INNER JOIN [Role] R ON R.[RoleID] = UR.RoleID WHERE R.[Name] = @RoleName;", new
                    {
                        RoleName = roleName
                    });
                }

                public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    var result = await this.Connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [UserPassword] WHERE UserID=@UserID AND IsDefault = 1", new
                    {
                        user.UserID
                    });

                    return result > 0;
                }

                public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    return await this.Connection.ExecuteAsync("UPDATE [UserPassword] SET BadPasswordCount = BadPasswordCount + 1, BadPasswordDateTime=@BadPasswordDateTime WHERE [UserID] = @UserID AND IsDefault = 1;", new
                    {
                        user.UserID,
                        BadPasswordDateTime = DateTimeOffset.Now
                    });
                }

                public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
                {
                    throw new NotImplementedException();
                }

                public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
                {
                    throw new NotImplementedException();
                }

                public async Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    await this.Connection.ExecuteAsync("UPDATE [UserPassword] SET BadPasswordCount = 0, BadPasswordDateTime=NULL WHERE [UserID] = @UserID AND IsDefault = 1;", new
                    {
                        user.UserID
                    });

                }

                public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
                {
                    user.IsLockedOut = enabled;
                    return Task.FromResult<object>(null);
                }

                public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
                {
                    user.LockoutExpires = lockoutEnd;

                    return Task.FromResult<object>(null);
                }

                public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
                {
                    user.UserName = normalizedName;

                    return Task.FromResult<object>(null);
                }

                public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
                {
                    var result = await this.Connection.ExecuteAsync("UPDATE [UserPassword] SET [Password] = @Password WHERE UserID=@UserID AND IsDefault = 1", new
                    {
                        user.UserID,
                        Password = passwordHash
                    });

                    if (result == 0)
                    {
                        await this.Connection.ExecuteAsync("INSERT INTO [UserPassword](UserID,Password,IsDefault) VALUES(@UserID,@Password,1)", new
                        {
                            user.UserID,
                            Password = passwordHash
                        });
                    }
                }

                public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
                {
                    user.UserName = userName;
                    return Task.FromResult<object>(null);
                }

                public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
                {
                    var result = await this.Connection.ExecuteAsync($"UPDATE {this.UserTableName} SET UserName=@UserName,Email=@Email,DisplayName=@DisplayName,TwoFactorEnabled=@TwoFactorEnabled,IsLockedOut=@IsLockedOut,LockoutExpires=@LockoutExpires WHERE UserID=@UserID;", param: user);

                    return result == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError() { Code = "UpdateFailed", Description = "Unexpected rowcount from update." });
                }

                */
    }

    
}
