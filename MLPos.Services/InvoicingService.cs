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
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IProductRepository _productRepository;

        private readonly IDbContext _dbContext;

        public InvoicingService(
            IInvoiceHeaderRepository invoiceHeaderRepository,
            IInvoiceLineRepository invoiceLineRepository,
            IPostedTransactionHeaderRepository postedTransactionHeaderRepository,
            ICustomerRepository customerRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IProductRepository productRepository,
            IDbContext dbContext)
        {
            _invoiceHeaderRepository = invoiceHeaderRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _postedTransactionHeaderRepository = postedTransactionHeaderRepository;
            _customerRepository = customerRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _productRepository = productRepository;
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

        public async Task<InvoiceHeader> GetInvoiceAsync(long invoiceId)
        {
            InvoiceHeader invoiceHeader = await _invoiceHeaderRepository.GetInvoiceHeaderAsync(invoiceId);
            invoiceHeader.Customer = await _customerRepository.GetCustomerAsync(invoiceHeader.Customer.Id);
            invoiceHeader.PaymentMethod = await _paymentMethodRepository.GetPaymentMethodAsync(invoiceHeader.PaymentMethod.Id);
            invoiceHeader.Lines = await _invoiceLineRepository.GetInvoiceLinesAsync(invoiceId);

            foreach (InvoiceLine line in invoiceHeader.Lines)
            {
                line.Product = await _productRepository.GetProductAsync(line.Product.Id);
            }

            return invoiceHeader;
        }

        public async Task<IEnumerable<InvoiceHeader>> GetInvoicesAsync(int limit, int offset)
        {
            IEnumerable<InvoiceHeader> invoiceHeaders = await _invoiceHeaderRepository.GetInvoiceHeadersAsync(limit, offset);

            foreach (InvoiceHeader invoice in invoiceHeaders)
            {
                invoice.Customer = await _customerRepository.GetCustomerAsync(invoice.Customer.Id);
                invoice.PaymentMethod = await _paymentMethodRepository.GetPaymentMethodAsync(invoice.PaymentMethod.Id);
            }

            return invoiceHeaders;
        }
    }
}
