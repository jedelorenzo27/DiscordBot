using BotShared.models;
using Dapper;
using System.Data;
using MySqlConnector;

namespace BotDataAccess.repositories
{
    public class ChallengeSubmissionRepo
    {
        private readonly IDbConnection _db;

        public ChallengeSubmissionRepo(string connectionString)
        {
            _db = new MySqlConnection(connectionString);
        }

        public async Task<List<ChallengeSubmissionModel>> GetSubmissionsForChallenge(string challengeId)
        {
            var sql = $"SELECT * FROM ChallengeSubmissions WHERE ChallengeId = '{challengeId}';";
            SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
            return (await results.ReadAsync<ChallengeSubmissionModel>()).ToList();
        }

        public async Task<int> AddSubmission(string challengeId, string userId, TimeComplexity BigO, Language ProgrammingLanguage, DateTime subscribeDate)
        {
            var sql = $"INSERT INTO ChallengeSubmissions VALUES ('{challengeId}', '{userId}', '{BigO}', '{ProgrammingLanguage}', '{subscribeDate}');";
            return await _db.ExecuteAsync(sql);
        }

        public async Task<int> RemoveSubmission(string challengeId, string userId, DateTime subscribeDate)
        {
            var sql = $"DELETE FROM ChallengeSubmissions WHERE ChallengeId = '{challengeId}' AND UserId = '{userId}';";
            return await _db.ExecuteAsync(sql);
        }
    }
}
