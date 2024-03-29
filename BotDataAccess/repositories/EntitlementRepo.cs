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




        public async Task<EntitlementModel[]> GetEntitlementsById(string id) // could be User/Server/Channel/Thread Id
        {
            ulong longId;
            if (ulong.TryParse(id, out longId))
            {
                return await GetEntitlementsById(longId);
            }
            return null;
        }
        public async Task<EntitlementModel[]> GetEntitlementsById(ulong id) // could be User/Server/Channel/Thread Id
        {
            var sql = "SELECT * FROM Entitlements WHERE Id = @Id;";
            return await _db.QuerySingleOrDefaultAsync<EntitlementModel[]>(sql, new { Id = id });
        }


        public async Task<EntitlementModel[]> GetEntitlementsByIds(ulong[] ids, Entitlement entitlement) // could be User/Server/Channel/Thread Id
        {
            List<string> idWhereConditions = new List<string>();
            foreach(ulong id in ids)
            {
                idWhereConditions.Add($"Id = {id}");
            }

            var sql = $"SELECT * FROM Entitlements WHERE ({string.Join(" OR ", idWhereConditions)}) AND Entitlment = @Entitlment;";
            return await _db.QuerySingleOrDefaultAsync<EntitlementModel[]>(sql, new { Entitlment = entitlement });
        }


        public async Task<EntitlementModel> GetEntitlementsById(string id, Entitlement entitlement)
        {
            ulong longId;
            if (ulong.TryParse(id, out longId))
            {
                return await GetEntitlementsById(longId, entitlement);
            }
            return null;
        }
        public async Task<EntitlementModel> GetEntitlementsById(ulong id, Entitlement entitlement)
        {
            var sql = "SELECT * FROM Entitlements WHERE Id = @Id AND Entitlment = @Entitlment;";
            return await _db.QuerySingleOrDefaultAsync<EntitlementModel>(sql, new { Id = id , Entitlment = entitlement });
        }


        public async Task AddEntitlement(string id, Entitlement entitlement)
        {
            ulong longId;
            if (ulong.TryParse(id, out longId)) {
                await AddEntitlement(longId, entitlement);
            }
        }
        public async Task AddEntitlement(ulong id, Entitlement entitlement)
        {
            if (await GetEntitlementsById(id, entitlement) == null)
            {
                var sql = "INSERT INTO Entitlements (@Id, @Entitlment, @CreationDate);";
                await _db.QuerySingleOrDefaultAsync<EntitlementModel>(sql, new { Id = id, Entitlment = entitlement, CreationDate = DateTime.Now });
            }
        }


        public async Task RemoveEntitlement(string id, Entitlement entitlement)
        {
            ulong longId;
            if (ulong.TryParse(id, out longId))
            {
                await RemoveEntitlement(longId, entitlement);
            }
        }
        public async Task RemoveEntitlement(ulong id, Entitlement entitlement)
        {
            var sql = "DELETE FROM Entitlements WHERE Id = @Id AND Entitlment = @Entitlment;";
            await _db.QuerySingleOrDefaultAsync<EntitlementModel>(sql, new { Id = id, Entitlment = entitlement });
        }


        public async Task RemoveAllEntitlements(string id)
        {
            ulong longId;
            if (ulong.TryParse(id, out longId))
            {
                await RemoveAllEntitlements(longId);
            }
        }
        public async Task RemoveAllEntitlements(ulong id)
        {
            var sql = "DELETE FROM Entitlements WHERE Id = @Id;";
            await _db.QuerySingleOrDefaultAsync<EntitlementModel>(sql, new { Id = id });
        }

    }
}
