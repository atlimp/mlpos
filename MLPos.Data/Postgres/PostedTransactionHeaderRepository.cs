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
                @"INSERT INTO POSTEDTRANSACTIONHEADER(id, status, posclient_id, customer_id, paymentmethod_id, invoice_id)
                    VALUES (@id, @status, @posclient_id, @customer_id, @paymentmethod_id, @invoice_id) RETURNING id, status, posclient_id, customer_id, paymentmethod_id, invoice_id, date_inserted, date_updated",
                MapToPostedTransactionHeader,
                new Dictionary<string, object>() {
                    ["@id"] = transactionHeader.Id,
                    ["@status"] = (int)transactionHeader.Status,
                    ["@posclient_id"] = transactionHeader.PosClientId,
                    ["@customer_id"] = transactionHeader.Customer.Id,
                    ["@paymentmethod_id"] = transactionHeader.PaymentMethod?.Id,
                    ["@invoice_id"] = transactionHeader.InvoiceId
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
                            "SELECT id, status, posclient_id, customer_id, paymentmethod_id, invoice_id, date_inserted, date_updated FROM POSTEDTRANSACTIONHEADER WHERE id = @id AND posclient_id = @posclient_id",
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
                "SELECT id, status, posclient_id, customer_id, paymentmethod_id, invoice_id, date_inserted, date_updated FROM POSTEDTRANSACTIONHEADER ORDER BY date_inserted DESC LIMIT @limit OFFSET @offset",
                MapToPostedTransactionHeader,
                new Dictionary<string, object>()
                {
                    ["@limit"] = limit,
                    ["@offset"] = offset,
                }
            );
        }

        public async Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersAsync(PostedTransactionQueryFilter filter)
        {
            string query = "SELECT id, status, posclient_id, customer_id, paymentmethod_id, invoice_id, date_inserted, date_updated FROM POSTEDTRANSACTIONHEADER WHERE ";
            Tuple<string, Dictionary<string, object>> parsed = ParseQueryFilter(filter);
            query = query + parsed.Item1;
            return await this.ExecuteQuery(
                            query,
                            MapToPostedTransactionHeader,
                            parsed.Item2
                        );
        }

        public async Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersAsync(PostedTransactionQueryFilter filter, int limit, int offset)
        {
            string query = "SELECT id, status, posclient_id, customer_id, paymentmethod_id, invoice_id, date_inserted, date_updated FROM POSTEDTRANSACTIONHEADER WHERE ";
            Tuple<string, Dictionary<string, object>> parsed = ParseQueryFilter(filter);
            query = query + parsed.Item1;
            query = query + "ORDER BY date_inserted DESC LIMIT @limit OFFSET @offset";

            parsed.Item2["@limit"] = limit;
            parsed.Item2["@offset"] = offset;

            return await this.ExecuteQuery(
                            query,
                            MapToPostedTransactionHeader,
                            parsed.Item2
                        );
        }

        public async Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersForInvoiceAsync(long invoiceId)
        {
            return await this.ExecuteQuery(
                            "SELECT id, status, posclient_id, customer_id, paymentmethod_id, invoice_id, date_inserted, date_updated FROM POSTEDTRANSACTIONHEADER WHERE invoice_id = @invoice_id",
                            MapToPostedTransactionHeader,
                            new Dictionary<string, object>()
                            {
                                ["@invoice_id"] = invoiceId,
                            }
                        );
        }

        public async Task<PostedTransactionHeader> UpdatePostedTransactionHeaderAsync(PostedTransactionHeader transactionHeader)
        {
            IEnumerable<PostedTransactionHeader> transactionHeaders = await this.ExecuteQuery(
                            "UPDATE POSTEDTRANSACTIONHEADER set status = @status, invoice_id = @invoice_id WHERE id = @id AND posclient_id = @posclient_id RETURNING id, status, posclient_id, customer_id, paymentmethod_id, invoice_id, date_inserted, date_updated",
                            MapToPostedTransactionHeader,
                            new Dictionary<string, object>()
                            {
                                ["@id"] = transactionHeader.Id,
                                ["@status"] = (int)transactionHeader.Status,
                                ["@posclient_id"] = transactionHeader.PosClientId,
                                ["@invoice_id"] = transactionHeader.InvoiceId,
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
                InvoiceId = reader.GetSafeInt64(5),
                DateInserted = reader.GetDateTime(6),
                DateUpdated = reader.GetDateTime(7),
            };
        }

        private Tuple<string, Dictionary<string, object>> ParseQueryFilter(PostedTransactionQueryFilter queryFilter)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> filters = new Dictionary<string, object>();
            sb.Append("1=1 ");

            if (queryFilter != null)
            {
                if (queryFilter.Period != null)
                {
                    sb.Append("AND date_inserted >= @period_from ");
                    filters["@period_from"] = queryFilter.Period.DateFrom;
                    sb.Append("AND date_inserted <= @period_to ");
                    filters["@period_to"] = queryFilter.Period.DateTo;
                }

                if (queryFilter.Status != null)
                {
                    sb.Append("AND status = @status ");
                    filters["@status"] = (int)queryFilter.Status;
                }

                if (queryFilter.CustomerId != null)
                {
                    sb.Append("AND customer_id = @customer_id ");
                    filters["@customer_id"] = queryFilter.CustomerId;
                }

                if (queryFilter.PaymentMethodId != null)
                {
                    sb.Append("AND paymentmethod_id = @paymentmethod_id ");
                    filters["@paymentmethod_id"] = queryFilter.PaymentMethodId;
                }
            }

            return new Tuple<string, Dictionary<string, object>>(sb.ToString(), filters);
        }
    }
}
