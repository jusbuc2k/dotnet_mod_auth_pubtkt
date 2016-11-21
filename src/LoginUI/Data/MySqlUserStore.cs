using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace WebApplication.Data
{

    public class MySqlUserStoreOptions
    {
        public string ConnectionString { get; set; }
        public string Query { get; set; }
        public System.Data.CommandType CommandType { get; set; }
        public string HashType { get; set; }
    }

    public class MySqlUserStore : DbUserStore
    {
        public MySqlUserStore(string connectionString) : base(new MySql.Data.MySqlClient.MySqlConnection(connectionString))
        {
        }

        public override bool ValidatePassword(string username, string password)
        {
            var results = this.Connection.Query<dynamic>("AuthenticateUser", 
                commandType: System.Data.CommandType.StoredProcedure,
                param: new
                {
                    vUsername = username,
                    vPassword = password,
                    AppName = "mirkweb",
                    OutFormat = 1,
                    vIgnoreWhitespace = false
                }
            );

            return results.Any();
        }
    }
}
