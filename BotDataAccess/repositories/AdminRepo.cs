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
    public  class AdminRepo
    {
        private readonly IDbConnection _db;

        public AdminRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }

        public async Task DeleteAllData()
        {
            var sql = $"DELETE FROM Users;";
            await _db.ExecuteAsync(sql);

            sql = $"DELETE FROM UsageStats;";
            await _db.ExecuteAsync(sql);

            sql = $"DELETE FROM Challenges;";
            await _db.ExecuteAsync(sql);

            sql = $"DELETE FROM ChallengeSubmissions;";
            await _db.ExecuteAsync(sql);

            sql = $"DELETE FROM ChallengeSubscribers;";
            await _db.ExecuteAsync(sql);
        }
    }
}
