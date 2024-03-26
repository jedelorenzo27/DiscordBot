using BotShared.models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotDataAccess.repositories
{
    public class UserRepo
    {
        private readonly IDbConnection _db;

        public UserRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }

        public async Task<UserModel> GetUserbyId(ulong userId)
        {
            var sql = "SELECT * FROM Users WHERE Users = @Users;";
            return await _db.QuerySingleOrDefaultAsync<UserModel>(sql, new { UserId = userId });
        }
    }
}
