using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotDataAccess.repositories
{
    public class ChallengeSubscriberRepo
    {
        private readonly IDbConnection _db;

        public ChallengeSubscriberRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }
    }
}
