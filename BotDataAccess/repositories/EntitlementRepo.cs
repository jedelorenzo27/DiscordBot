using BotShared;
using BotShared.models;
using Dapper;
using MySqlConnector;  

using System.Data;

namespace BotDataAccess.repositories
{
    public class EntitlementRepo
    {
        private readonly IDbConnection _db;

        public EntitlementRepo(string connectionString)
        {
            _db = new MySqlConnection(connectionString);
        }

        public async Task<List<EntitlementModel>> GetEntitlementsById(string id) 
        {
            try
            {
                var sql = $"SELECT * FROM Entitlements WHERE Id = '{id}';";
                SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
                return (await results.ReadAsync<EntitlementModel>()).ToList();
            }
            catch (Exception ex)
            {
                string[] errors = new string[] {
                    $"[GetEntitlementsById] Failed while retrieving entitlements by Id",
                    $"[GetEntitlementsById] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
                throw;
            }
        }

        public async Task<List<EntitlementModel>> GetEntitlementsByIds(string[] ids, Entitlement entitlement)
        {
            try
            {
                List<string> idWhereConditions = new List<string>();
                foreach (string id in ids)
                {
                    idWhereConditions.Add($"Id = '{id}'");
                }

                var sql = $"SELECT * FROM Entitlements WHERE Entitlement = '{entitlement}' AND ({string.Join(" OR ", idWhereConditions)});";
                SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
                List<EntitlementModel> list = (await results.ReadAsync<EntitlementModel>()).ToList();
                return list;
            }
            catch (Exception ex)
            {
                string[] errors = new string[] {
                    $"[GetEntitlementsByIds] Failed while retrieving entitlements by Ids and entitlement",
                    $"[GetEntitlementsByIds] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
                throw;
            }
        }

        public async Task<EntitlementModel> GetEntitlementById(string id, Entitlement entitlement)
        {
            try { 
                var sql = $"SELECT * FROM Entitlements WHERE Id = '{id}' AND Entitlement = '{entitlement}';";
                return await _db.QueryFirstOrDefaultAsync<EntitlementModel>(sql);
            }
            catch (Exception ex)
            {
                string[] errors = new string[] {
                    $"[GetEntitlementById] Failed while retrieving entitlements by Id and entitlement",
                    $"[GetEntitlementById] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
                throw;
            }
}

        public async Task AddEntitlement(string id, Entitlement entitlement)
        {
            if (await GetEntitlementById(id, entitlement) == null)
            {
                try { 
                    var sql = $"INSERT INTO Entitlements VALUES ('{id}', '{entitlement}', '{DateTime.Now}');";
                    await _db.ExecuteAsync(sql);
                }
                catch (Exception ex)
                {
                    string[] errors = new string[] {
                        $"[AddEntitlement] Failed while storing id:{id}, entitlement:{entitlement}",
                        $"[AddEntitlement] Error: {ex.Message}"
                    };
                    await Logger.LogMessage(LogLevel.Error, errors);
                    throw;
                }
            }
        }

        public async Task RemoveEntitlement(string id, Entitlement entitlement)
        {
            try { 
                var sql = $"DELETE FROM Entitlements WHERE Id = '{id}' AND Entitlement = '{entitlement}';";
                await _db.ExecuteAsync(sql);
            }
            catch (Exception ex)
            {
                string[] errors = new string[] {
                    $"[RemoveEntitlement] Failed while removing entitlement, id:{id}, entitlement:{entitlement}",
                    $"[RemoveEntitlement] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
                throw;
            }
}

        public async Task RemoveAllEntitlements(string id)
        {
            try
            {
                var sql = $"DELETE FROM Entitlements WHERE Id = '{id}';";
                await _db.ExecuteAsync(sql);
            }
            catch (Exception ex)
            {
                string[] errors = new string[] {
                    $"[RemoveAllEntitlements] Failed while removing all entitlements under id:{id}",
                    $"[RemoveAllEntitlements] Error: {ex.Message}"
                };
                await Logger.LogMessage(LogLevel.Error, errors);
                throw;
            }
        }
    }
}