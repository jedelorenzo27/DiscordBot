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

        public async Task<ChallengeModel> GetByIdAsync(int leetcodeId)
        {
            var sql = "SELECT * FROM Challenges WHERE LeetcodeId = @LeetcodeId;";
            return await _db.QuerySingleOrDefaultAsync<ChallengeModel>(sql, new { LeetcodeId = leetcodeId });

        }

        public async Task<int> AddAsync(ChallengeModel challengeModel)
        {
            var sql = @"INSERT INTO Challenges (ServerId, ChannelId, CreationDate, LeetcodeName, LeetcodeId) 
            VALUES (@ServerId, @ChannelId, @CreationDate, @LeetcodeName, @LeetcodeId);";

            return await _db.ExecuteAsync(sql, challengeModel);

        }
    }
}
