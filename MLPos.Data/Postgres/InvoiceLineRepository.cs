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
using System.Transactions;

namespace MLPos.Data.Postgres
{
    public class InvoiceLineRepository : RepositoryBase, IInvoiceLineRepository
    {
        public InvoiceLineRepository(string connectionString) : base(connectionString) { }
        public async Task<InvoiceLine> CreateInvoiceLineAsync(long invoiceId, InvoiceLine line)
        {
            IEnumerable<InvoiceLine> invoiceLines = await this.ExecuteQuery(
                                        @"INSERT INTO INVOICELINE(invoice_id, product_id, quantity, amount)
                                            VALUES (@invoice_id, @product_id, @quantity, @amount) RETURNING id, product_id, quantity, amount, date_inserted",
                                        MapToInvoiceLine,
                                        new Dictionary<string, object>()
                                        {
                                            ["@invoice_id"] = invoiceId,
                                            ["@product_id"] = line.Product.Id,
                                            ["@quantity"] = line.Quantity,
                                            ["@amount"] = line.Amount,
                                        }
                                    );

            if (invoiceLines.Any())
            {
                return invoiceLines.First();
            }

            return null;
        }

        public async Task<IEnumerable<InvoiceLine>> GetInvoiceLinesAsync(long invoiceId)
        {
            return await this.ExecuteQuery(
                            @"SELECT id, product_id, quantity, amount, date_inserted FROM INVOICELINE WHERE invoice_id = @invoice_id",
                            MapToInvoiceLine,
                            new Dictionary<string, object>()
                            {
                                ["@invoice_id"] = invoiceId,
                            }
                        );
        }

        public InvoiceLine MapToInvoiceLine(NpgsqlDataReader reader)
        {
            return new InvoiceLine
            {
                Id = reader.GetInt64(0),
                Product = new Product
                {
                    Id = reader.GetSafeInt64(1)
                },
                Quantity = reader.GetDecimal(2),
                Amount = reader.GetDecimal(3),
                DateInserted = reader.GetDateTime(4)
            };
        }
    }
}
