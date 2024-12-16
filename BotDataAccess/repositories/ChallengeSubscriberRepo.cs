using BotShared.models;
using Dapper;
using System.Data;
using MySqlConnector;

namespace BotDataAccess.repositories
{
    public class ChallengeSubscriberRepo
    {
        private readonly IDbConnection _db;

        public ChallengeSubscriberRepo(string connectionString)
        {
            _db = new MySqlConnection(connectionString);
        }

        public async Task<List<ChallengeSubscriberModel>> GetSubscribersByDiscordId(string discordId)
        {
            var sql = $"SELECT * FROM ChallengeSubscribers WHERE DiscordId = '{discordId}';";
            SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
            return (await results.ReadAsync<ChallengeSubscriberModel>()).ToList();
        }

        public async Task<ChallengeSubscriberModel> GetSubscriber(string discordId, string userId)
        {
            var sql = $"SELECT * FROM ChallengeSubscribers WHERE DiscordId = '{discordId}' AND UserId = '{userId}';";
            return await _db.QueryFirstOrDefaultAsync<ChallengeSubscriberModel>(sql);
        }

        public async Task<int> AddSubscriber(string discordId, string userId, DateTime subscribeDate)
        {
            if (await GetSubscriber(discordId, userId) == null ) 
            {
                var sql = $"INSERT INTO ChallengeSubscribers VALUES ('{discordId}', '{userId}', '{subscribeDate}');";
                return await _db.ExecuteAsync(sql);
            }
            return 0;
        }

        public async Task<int> RemoveSubscriber(string discordId, string userId)
        {
            var sql = $"DELETE FROM ChallengeSubscribers WHERE DiscordId = '{discordId}' AND UserId = '{userId}';";
            return await _db.ExecuteAsync(sql);
        }
    }
}
