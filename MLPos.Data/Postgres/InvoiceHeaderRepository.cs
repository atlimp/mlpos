using MLPos.Core.Enums;
using MLPos.Core.Exceptions;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Model;
using MLPos.Data.Postgres.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MLPos.Data.Postgres
{
    public class InvoiceHeaderRepository : RepositoryBase, IInvoiceHeaderRepository
    {
        public InvoiceHeaderRepository(string connectionString) : base(connectionString) { }

        public async Task<InvoiceHeader> CreateInvoiceHeaderAsync(InvoiceHeader invoiceHeader)
        {
            IEnumerable<InvoiceHeader> invoiceHeaders = await this.ExecuteQuery(
                            @"INSERT INTO INVOICEHEADER(status, customer_id, paymentmethod_id, period_from, period_to)
                    VALUES (@status, @customer_id, @paymentmethod_id, @period_from, @period_to) RETURNING id, status, customer_id, paymentmethod_id, period_from, period_to, date_inserted",
                            MapToInvoiceHeader,
                            new Dictionary<string, object>()
                            {
                                ["@status"] = (int)invoiceHeader.Status,
                                ["@customer_id"] = invoiceHeader.Customer.Id,
                                ["@paymentmethod_id"] = invoiceHeader.PaymentMethod?.Id,
                                ["@period_from"] = invoiceHeader.Period.DateFrom,
                                ["@period_to"] = invoiceHeader.Period.DateTo,
                            }
                        );

            if (invoiceHeaders.Any())
            {
                return invoiceHeaders.First();
            }

            return null;
        }

        public async Task<InvoiceHeader> GetInvoiceHeaderAsync(long invoiceId)
        {
            IEnumerable<InvoiceHeader> invoiceHeaders = await this.ExecuteQuery(
                                        "SELECT id, status, customer_id, paymentmethod_id, period_from, period_to, date_inserted FROM INVOICEHEADER WHERE id = @id",
                                        MapToInvoiceHeader,
                                        new Dictionary<string, object>()
                                        {
                                            ["@id"] = invoiceId,
                                        }
                                    );

            if (invoiceHeaders.Any())
            {
                return invoiceHeaders.First();
            }

            throw new EntityNotFoundException(typeof(InvoiceHeader), invoiceId);

        }

        public async Task<IEnumerable<InvoiceHeader>> GetInvoiceHeadersAsync(int limit, int offset)
        {
            return await this.ExecuteQuery(
                            "SELECT id, status, customer_id, paymentmethod_id, period_from, period_to, date_inserted FROM INVOICEHEADER ORDER BY date_inserted DESC LIMIT @limit OFFSET @offset",
                            MapToInvoiceHeader,
                            new Dictionary<string, object>()
                            {
                                ["@limit"] = limit,
                                ["@offset"] = offset,
                            }
                        );
        }
        public async Task<InvoiceHeader> UpdateInvoiceHeaderAsync(InvoiceHeader invoiceHeader)
        {
            IEnumerable<InvoiceHeader> invoiceHeaders = await this.ExecuteQuery(
                                        @"UPDATE INVOICEHEADER SET status = @status WHERE id = @id",
                                        MapToInvoiceHeader,
                                        new Dictionary<string, object>()
                                        {
                                            ["@status"] = (int)invoiceHeader.Status,
                                            ["@id"] = invoiceHeader.Id,
                                        }
                                    );

            if (invoiceHeaders.Any())
            {
                return invoiceHeaders.First();
            }

            return null;
        }

        public async Task<IEnumerable<InvoiceHeader>> GetInvoiceHeadersAsync(InvoiceQueryFilter queryFilter, int limit, int offset)
        {
            string query = "SELECT id, status, customer_id, paymentmethod_id, period_from, period_to, date_inserted FROM INVOICEHEADER WHERE ";
            Tuple<string, Dictionary<string, object>> parsed = ParseQueryFilter(queryFilter);
            query = query + parsed.Item1;
            query = query + "ORDER BY date_inserted DESC LIMIT @limit OFFSET @offset";

            parsed.Item2["@limit"] = limit;
            parsed.Item2["@offset"] = offset;

            return await this.ExecuteQuery(
                                        query,
                                        MapToInvoiceHeader,
                                        parsed.Item2
                                    );
        }
        public InvoiceHeader MapToInvoiceHeader(NpgsqlDataReader reader)
        {
            return new InvoiceHeader
            {
                Id = reader.GetInt64(0),
                Status = (InvoiceStatus)reader.GetInt32(1),
                Customer = new Customer
                {
                    Id = reader.GetSafeInt64(2)
                },
                PaymentMethod = new PaymentMethod
                {
                    Id = reader.GetSafeInt64(3)
                },
                Period = new Period
                {
                    DateFrom = reader.GetDateTime(4),
                    DateTo = reader.GetDateTime(5)
                },
                DateInserted = reader.GetDateTime(6)
            };
        }


        private Tuple<string, Dictionary<string, object>> ParseQueryFilter(InvoiceQueryFilter queryFilter)
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
