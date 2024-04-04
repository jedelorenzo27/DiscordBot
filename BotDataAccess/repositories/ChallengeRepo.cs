using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using BotShared.models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BotDataAccess.repositories
{
    public class ChallengeRepo
    {
        private readonly IDbConnection _db;

        public ChallengeRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }

        public async Task<List<ChallengeModel>> GetChallengeByChannelId(int channelId)
        {
            var sql = $"SELECT * FROM Challenges WHERE ChannelId = {channelId};";
            SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
            return (await results.ReadAsync<ChallengeModel>()).ToList();
        }

        public async Task<List<ChallengeModel>> GetChallengeByIdAsync(int challengeId)
        {
            var sql = $"SELECT * FROM Challenges WHERE ChallengeId = {challengeId};";
            SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
            return (await results.ReadAsync<ChallengeModel>()).ToList();
        }

        public async Task<int> AddChallenge(ChallengeModel challengeModel)
        {
            var sql = $"INSERT INTO Challenges VALUES ('{challengeModel.ChallengeId}', '{challengeModel.ChannelId}', '{challengeModel.ServerId}', '{challengeModel.CreationDate}', '{challengeModel.LeetcodeName}', '{challengeModel.LeetcodeNumber}', '{challengeModel.ChallengeDifficulty}');";
            return await _db.ExecuteAsync(sql, challengeModel);
        }
    }
}
