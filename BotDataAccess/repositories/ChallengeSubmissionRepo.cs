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
    public class ChallengeSubmissionRepo
    {
        private readonly IDbConnection _db;

        public ChallengeSubmissionRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }

        public async Task<List<ChallengeSubmissionModel>> GetSubmissionsForChallenge(string channelId)
        {
            var sql = $"SELECT * FROM ChallengeSubmissions WHERE ChannelId = '{channelId}';";
            SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
            return (await results.ReadAsync<ChallengeSubmissionModel>()).ToList();
        }

        public async Task<int> AddSubmission(string channelId, string userId, DateTime subscribeDate)
        {
            var sql = $"INSERT INTO ChallengeSubmissions VALUES ('{channelId}', '{userId}', '{subscribeDate}');";
            return await _db.ExecuteAsync(sql);
        }

        public async Task<int> RemoveSubmission(string channelId, string userId, DateTime subscribeDate)
        {
            var sql = $"DELETE FROM ChallengeSubmissions WHERE ChannelId = '{channelId}' AND UserId = '{userId}';";
            return await _db.ExecuteAsync(sql);
        }
    }
}
