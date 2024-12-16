using Dapper;
using System.Data;
using MySqlConnector;

namespace BotDataAccess.repositories
{
    public  class AdminRepo
    {
        private readonly IDbConnection _db;

        public AdminRepo(string connectionString)
        {
            _db = new MySqlConnection(connectionString);
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
