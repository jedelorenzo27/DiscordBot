using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        public async Task<ChallengeModel> GetChallengeByIdAsync(int challengeId)
        {
            var sql = "SELECT * FROM Challenges WHERE ChallengeId = @ChallengeId;";
            return await _db.QuerySingleOrDefaultAsync<ChallengeModel>(sql, new { ChallengeId = challengeId });
        }

        public async Task<int> AddAsync(ChallengeModel challengeModel)
        {
            var sql = @"INSERT INTO Challenges (ServerId, ChannelId, CreationDate, LeetcodeName, LeetcodeId) 
            VALUES (@ServerId, @ChannelId, @CreationDate, @LeetcodeName, @LeetcodeId);";

            return await _db.ExecuteAsync(sql, challengeModel);
        }
    }
}
