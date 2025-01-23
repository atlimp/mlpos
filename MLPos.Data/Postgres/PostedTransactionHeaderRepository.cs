using MLPos.Core.Enums;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Data.Postgres
{
    public class PostedTransactionHeaderRepository : RepositoryBase, IPostedTransactionHeaderRepository
    {
        public PostedTransactionHeaderRepository(string connectionString) : base(connectionString) { }

        public async Task<PostedTransactionHeader> CreatePostedTransactionHeaderAsync(PostedTransactionHeader transactionHeader)
        {
            IEnumerable<PostedTransactionHeader> transactionHeaders = await this.ExecuteQuery(
                "INSERT INTO POSTEDTRANSACTIONHEADER(id, status, posclient_id, customer_id, paymentmethod_id) VALUES (@id, @status, @posclient_id, @customer_id, @paymentmethod_id) RETURNING id, status, posclient_id, customer_id, paymentmethod_id, date_inserted, date_updated",
                MapToPostedTransactionHeader,
                new Dictionary<string, object>() {
                    ["@id"] = transactionHeader.Id,
                    ["@status"] = (int)transactionHeader.Status,
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

        public async Task<PostedTransactionHeader> GetPostedTransactionHeaderAsync(long transactionId, long posClientId)
        {
            IEnumerable<PostedTransactionHeader> transactionHeaders = await this.ExecuteQuery(
                            "SELECT id, status, posclient_id, customer_id, paymentmethod_id, date_inserted, date_updated FROM POSTEDTRANSACTIONHEADER WHERE id = @id AND posclient_id = @posclient_id",
                            MapToPostedTransactionHeader,
                            new Dictionary<string, object>()
                            {
                                ["@id"] = transactionId,
                                ["@posclient_id"] = posClientId,
                            }
                        );

            if (transactionHeaders.Any())
            {
                return transactionHeaders.First();
            }

            throw new EntityNotFoundException(typeof(PostedTransactionHeader), transactionId);
        }

        public async Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersAsync(int limit, int offset)
        {
            return await this.ExecuteQuery(
                "SELECT id, status, posclient_id, customer_id, paymentmethod_id, date_inserted, date_updated FROM POSTEDTRANSACTIONHEADER ORDER BY date_inserted DESC LIMIT @limit OFFSET @offset",
                MapToPostedTransactionHeader,
                new Dictionary<string, object>()
                {
                    ["@limit"] = limit,
                    ["@offset"] = offset,
                }
            );
        }

        private PostedTransactionHeader MapToPostedTransactionHeader(NpgsqlDataReader reader)
        {
            return new PostedTransactionHeader()
            {
                Id = reader.GetInt64(0),
                Status = (TransactionStatus)reader.GetInt32(1),
                PosClientId = reader.GetInt64(2),
                Customer = new Customer
                {
                    Id = reader.GetInt64(3)
                },
                PaymentMethod = new PaymentMethod
                {
                    Id = reader.GetSafeInt64(4)
                },
                DateInserted = reader.GetDateTime(5),
                DateUpdated = reader.GetDateTime(6),
            };
        }
    }
}
