using System.Data;
using BotShared;
using BotShared.models;
using Dapper;
using MySqlConnector;

namespace BotDataAccess.repositories
{
    public class ChallengeRepo
    {
        private readonly IDbConnection _db;

        public ChallengeRepo(string connectionString)
        {
            _db = new MySqlConnection(connectionString);
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
                string[] errors = new string[] {
                    $"[GetChallengesByServerId] Failed while retrieving challenges by server Id",
                    $"[GetChallengesByServerId] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
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
                string[] errors = new string[] {
                    $"[GetChallengeByChallengeId] Failed while retrieving challenge",
                    $"[GetChallengeByChallengeId] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
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
                string[] errors = new string[] {
                    $"[AddChallenge] Failed while storing challenge",
                    $"[AddChallenge] Challenge that failed to store: ({challengeModel})",
                    $"[AddChallenge] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
                return 0;
            }
        }
    }
}
