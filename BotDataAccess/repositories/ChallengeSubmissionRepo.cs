using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotDataAccess.repositories
{
    public class ChallengeSubmissionRepo
    {
        private readonly IDbConnection _db;

        public ChallengeSubmissionRepo(string connectionString)
        {
            _db = new SqlConnection(connectionString);
        }
    }
}
