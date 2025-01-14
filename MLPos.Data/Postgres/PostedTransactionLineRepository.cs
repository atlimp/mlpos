using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class PostedTransactionLineRepository : IPostedTransactionLineRepository
    {
        private readonly string _connectionString;

        public PostedTransactionLineRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PostedTransactionLine> CreatePostedTransactionLineAsync(long transactionId, long posClientId, PostedTransactionLine line)
        {
            throw new NotImplementedException();
        }
    }
}
