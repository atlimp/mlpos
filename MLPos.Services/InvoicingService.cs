using MLPos.Core.Enums;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Services
{
    public class InvoicingService : IInvoicingService
    {
        private readonly IInvoiceHeaderRepository _invoiceHeaderRepository;
        private readonly IInvoiceLineRepository _invoiceLineRepository;
        private readonly IPostedTransactionHeaderRepository _postedTransactionHeaderRepository;

        private readonly IDbContext _dbContext;

        public InvoicingService(
            IInvoiceHeaderRepository invoiceHeaderRepository,
            IInvoiceLineRepository invoiceLineRepository,
            IPostedTransactionHeaderRepository postedTransactionHeaderRepository,
            IDbContext dbContext)
        {
            _invoiceHeaderRepository = invoiceHeaderRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _postedTransactionHeaderRepository = postedTransactionHeaderRepository;
            _dbContext = dbContext;
        }
    
        public async Task<InvoiceHeader> GenerateInvoice(PostedTransactionHeader transactionHeader)
        {
            _invoiceHeaderRepository.SetDBContext(_dbContext);
            _invoiceLineRepository.SetDBContext(_dbContext);
            _postedTransactionHeaderRepository.SetDBContext(_dbContext);

            try
            {
                _dbContext.BeginDbTransaction();
                InvoiceHeader invoiceHeader = await _invoiceHeaderRepository.CreateInvoiceHeaderAsync(new InvoiceHeader
                {
                    Status = InvoiceStatus.Invoiced,
                    Customer = transactionHeader.Customer,
                    PaymentMethod = transactionHeader.PaymentMethod,
                    Period = new Period
                    {
                        DateFrom = transactionHeader.DateInserted.Date,
                        DateTo = transactionHeader.DateInserted.Date,
                    }
                });

                List<InvoiceLine> invoiceLines = new List<InvoiceLine>();
                foreach (PostedTransactionLine line in transactionHeader.Lines)
                {
                    InvoiceLine invoiceLine = await _invoiceLineRepository.CreateInvoiceLineAsync(invoiceHeader.Id, new InvoiceLine
                    {
                        Product = line.Product,
                        Quantity = line.Quantity,
                        Amount = line.Amount,
                    });

                    invoiceLines.Add(invoiceLine);
                }

                invoiceHeader.Lines = invoiceLines;

                transactionHeader.Status = TransactionStatus.Invoiced;
                transactionHeader.InvoiceId = invoiceHeader.Id;
                await _postedTransactionHeaderRepository.UpdatePostedTransactionHeaderAsync(transactionHeader);

                _dbContext.CommitDbTransaction();

                return invoiceHeader;
            }
            catch (Exception ex)
            {
                _dbContext.RollbackDbTransaction();
                throw;
            }
        }

        public Task<InvoiceHeader> GenerateInvoice(Customer customer, PaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }
    }
}
