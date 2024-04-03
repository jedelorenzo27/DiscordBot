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
    public class EntitlementRepo
    {
        private readonly IDbConnection _db;

        public EntitlementRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }

        public async Task<List<EntitlementModel>> GetEntitlementsById(string id) 
        {
            var sql = $"SELECT * FROM Entitlements WHERE Id = {id};";
            SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
            return (await results.ReadAsync<EntitlementModel>()).ToList();
        }

        public async Task<List<EntitlementModel>> GetEntitlementsByIds(string[] ids, Entitlement entitlement)
        {
            List<string> idWhereConditions = new List<string>();
            foreach(string id in ids)
            {
                idWhereConditions.Add($"Id = {id}");
            }

            var sql = $"SELECT * FROM Entitlements WHERE Entitlement = {entitlement} AND ({string.Join(" OR ", idWhereConditions)});";
            SqlMapper.GridReader results = await _db.QueryMultipleAsync(sql);
            return (await results.ReadAsync<EntitlementModel>()).ToList();
        }

        public async Task<EntitlementModel> GetEntitlementById(string id, Entitlement entitlement)
        {
            var sql = $"SELECT * FROM Entitlements WHERE Id = '{id}' AND Entitlement = '{entitlement}';";
            return await _db.QueryFirstOrDefaultAsync<EntitlementModel>(sql);
        }

        public async Task AddEntitlement(string id, Entitlement entitlement)
        {
            if (await GetEntitlementById(id, entitlement) != null)
            {
                var sql = $"INSERT INTO Entitlements VALUES ('{id}', '{entitlement}', '{DateTime.Now}');";
                await _db.QueryAsync(sql);
            }
        }

        public async Task RemoveEntitlement(string id, Entitlement entitlement)
        {
            var sql = $"DELETE FROM Entitlements WHERE Id = {id} AND Entitlment = {entitlement};";
            await _db.QuerySingleOrDefaultAsync<EntitlementModel>(sql);
        }

        public async Task RemoveAllEntitlements(string id)
        {
            var sql = $"DELETE FROM Entitlements WHERE Id = {id};";
            await _db.QuerySingleOrDefaultAsync<EntitlementModel>(sql, new { Id = id });
        }
    }
}