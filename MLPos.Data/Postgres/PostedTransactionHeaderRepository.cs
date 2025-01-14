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
    public class PostedTransactionHeaderRepository : IPostedTransactionHeaderRepository
    {
        private readonly string _connectionString;
        public PostedTransactionHeaderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PostedTransactionHeader> CreatePostedTransactionHeaderAsync(PostedTransactionHeader transactionHeader)
        {
            IEnumerable<PostedTransactionHeader> transactionHeaders = await SqlHelper.ExecuteQuery(_connectionString,
                "INSERT INTO POSTEDTRANSACTIONHEADER(id, posclient_id, customer_id, paymentmethod_id) VALUES (@id, @posclient_id, @customer_id, @paymentmethod_id) RETURNING id, posclient_id, customer_id, paymentmethod_id, date_inserted, date_updated",
                MapToPostedTransactionHeader,
                new Dictionary<string, object>() {
                    ["@id"] = transactionHeader.Id,
                    ["@posclient_id"] = transactionHeader.PosClientId,
                    ["@customer_id"] = transactionHeader.Customer.Id,
                    ["@paymentmethod_id"] = transactionHeader.PaymentMethod?.Id
                }
            );

            if (transactionHeaders.Any())
            {
                return transactionHeaders.First();
            }

            return null;
        }

        private PostedTransactionHeader MapToPostedTransactionHeader(NpgsqlDataReader reader)
        {
            return new PostedTransactionHeader()
            {
                Id = reader.GetInt64(0),
                PosClientId = reader.GetInt64(1),
                Customer = new Customer
                {
                    Id = reader.GetInt64(2)
                },
                PaymentMethod = new PaymentMethod
                {
                    Id = reader.GetSafeInt64(3)
                },
                DateInserted = reader.GetDateTime(4),
                DateUpdated = reader.GetDateTime(5),
            };
        }
    }
}
