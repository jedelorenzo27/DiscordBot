using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using BotShared;
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

        public async Task<List<ChallengeModel>> GetChallengesByServerId(string serverId)
        {
            try { 
                var sql = $"SELECT * FROM Challenges WHERE ServerId = '{serverId}';";
                SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
                return (await results.ReadAsync<ChallengeModel>()).ToList();
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(LogLevel.Error, $"[GetChallengesByServerId] Failed while retrieving challenges by server Id");
                await Logger.LogMessage(LogLevel.Error, $"[GetChallengesByServerId] Error: {ex.Message}");
                throw;
            }
        }

        public async Task<ChallengeModel> GetChallengeByChallengeId(string challengeId)
        {
            try { 
                var sql = $"SELECT * FROM Challenges WHERE ChallengeId = '{challengeId}';";
                return await _db.QueryFirstOrDefaultAsync<ChallengeModel>(sql);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(LogLevel.Error, $"[GetChallengeByChallengeId] Failed while retrieving challenge");
                await Logger.LogMessage(LogLevel.Error, $"[GetChallengeByChallengeId] Error: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddChallenge(ChallengeModel challengeModel)
        {
            try
            {
                int numberOfRowsAffected = 0;
                var sql = $"INSERT INTO Challenges VALUES ('{challengeModel.ChallengeId}', '{challengeModel.ServerId}', '{challengeModel.CreationDate}', '{challengeModel.LeetcodeName}', '{challengeModel.LeetcodeNumber}', '{challengeModel.ChallengeDifficulty}');";
                numberOfRowsAffected = await _db.ExecuteAsync(sql, challengeModel);
                await Logger.LogMessage(LogLevel.DBExecute, $"[AddChallenge] Added challenge {challengeModel}. Rows affected: {numberOfRowsAffected}");
                return numberOfRowsAffected;
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(LogLevel.Error, $"[AddChallenge] Failed while storing challenge");
                await Logger.LogMessage(LogLevel.Error, $"[AddChallenge] Challenge that failed to store: ({challengeModel})");
                await Logger.LogMessage(LogLevel.Error, $"[AddChallenge] Error: {ex.Message}");
                return 0;
            }
        }
    }
}
