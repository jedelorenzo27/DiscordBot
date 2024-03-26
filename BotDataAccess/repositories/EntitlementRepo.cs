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


        public async Task<EntitlementModel[]> GetEntitlementsById(int id) // could be User/Server/Channel/Thread Id
        {
            var sql = "SELECT * FROM Entitlements WHERE Id = @Id;";
            return await _db.QuerySingleOrDefaultAsync<EntitlementModel[]>(sql, new { Id = id });
        }

        public async Task<EntitlementModel> GetEntitlementsById(int id, Entitlement entitlement)
        {
            var sql = "SELECT * FROM Entitlements WHERE Id = @Id AND Entitlment = @Entitlment;";
            return await _db.QuerySingleOrDefaultAsync<EntitlementModel>(sql, new { Id = id , Entitlment = entitlement });
        }

    }
}
