using MLPos.Core.Enums;
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
    }
}
