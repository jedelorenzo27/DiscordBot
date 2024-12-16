using BotShared.models;
using Dapper;
using System.Data;
using MySqlConnector;

namespace BotDataAccess.repositories
{
    public class UserRepo
    {
        private readonly IDbConnection _db;

        public UserRepo(string connectionString)
        {
            _db = new MySqlConnection(connectionString);
        }

        public async Task<UserModel> GetUserbyId(string userId)
        {
            var sql = $"SELECT * FROM Users WHERE Users = '{userId}';";
            return await _db.QuerySingleOrDefaultAsync<UserModel>(sql);
        }

        public async Task<int> AddUser(string userId, UserType userType, string name)
        {
            if (await GetUserbyId(userId) == null)
            {
                var sql = $"INSERT INTO ChallengeSubscribers VALUES ('{userId}', '{userType}', '{name}', '0');";
                return await _db.ExecuteAsync(sql);
            }
            return 0;
        }
    }
}
