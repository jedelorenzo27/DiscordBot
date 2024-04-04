using BotShared.models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BotDataAccess.repositories
{
    public class UsageStatsRepo
    {

        private readonly IDbConnection _db;

        public UsageStatsRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }
 
        public async Task <int> IncrementStat(string id, string statType, StatTypeSuffix statTypeSuffix)
        {
            string joinedStatType = $"{statType}-{statTypeSuffix}";
            var sql = $"SELECT * FROM UsageStats WHERE Id = '{id}' AND StatType = '{joinedStatType}';";
            UsageStatsModel existingModel = await _db.QueryFirstOrDefaultAsync<UsageStatsModel>(sql);
            if (existingModel != null)
            {
                sql = $"UPDATE UsageStats SET TimesUsed = '{existingModel.TimesUsed+1}' WHERE Id = '{id}' AND StatType = '{joinedStatType}';";
                return await _db.ExecuteAsync(sql);
            } else
            {
                sql = $"INSERT INTO UsageStats VALUES ('{id}', '{joinedStatType}', '1');";
                return await _db.ExecuteAsync(sql);
            }
        }
    }
}
