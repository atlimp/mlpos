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
        private readonly IPostedTransactionLineRepository _postedTransactionLineRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IProductRepository _productRepository;

        private readonly IDbContext _dbContext;

        public InvoicingService(
            IInvoiceHeaderRepository invoiceHeaderRepository,
            IInvoiceLineRepository invoiceLineRepository,
            IPostedTransactionHeaderRepository postedTransactionHeaderRepository,
            IPostedTransactionLineRepository postedTransactionLineRepository,
            ICustomerRepository customerRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IProductRepository productRepository,
            IDbContext dbContext)
        {
            _invoiceHeaderRepository = invoiceHeaderRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _postedTransactionHeaderRepository = postedTransactionHeaderRepository;
            _postedTransactionLineRepository = postedTransactionLineRepository;
            _customerRepository = customerRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _productRepository = productRepository;
            _dbContext = dbContext;
        }
    
        public async Task<InvoiceHeader> GenerateInvoice(PostedTransactionHeader transactionHeader)
        {
            Period period = new Period
            {
                DateFrom = transactionHeader.DateInserted.Date,
                DateTo = transactionHeader.DateInserted.Date,
            };

            return await GenerateInvoice(transactionHeader.Customer, transactionHeader.PaymentMethod, period, new List<PostedTransactionHeader> { transactionHeader });
        }

        public async Task<InvoiceHeader> GenerateInvoice(Customer customer, PaymentMethod paymentMethod, Period period)
        {
            PostedTransactionQueryFilter filter = new PostedTransactionQueryFilter
            {
                CustomerId = customer.Id,
                PaymentMethodId = paymentMethod.Id,
                Period = period,
                Status = TransactionStatus.Posted
            };

            IEnumerable<PostedTransactionHeader> transactions = await _postedTransactionHeaderRepository.GetPostedTransactionHeadersAsync(filter);
            foreach (PostedTransactionHeader transaction in transactions)
            {
                transaction.Customer = await _customerRepository.GetCustomerAsync(transaction.Customer.Id);
                transaction.PaymentMethod = await _paymentMethodRepository.GetPaymentMethodAsync(transaction.PaymentMethod.Id);
                transaction.Lines = await _postedTransactionLineRepository.GetPostedTransactionLinesAsync(transaction.Id, transaction.PosClientId);
            }

            return await GenerateInvoice(customer, paymentMethod, period, transactions);
        }

        private async Task<InvoiceHeader> GenerateInvoice(Customer customer, PaymentMethod paymentMethod, Period period, IEnumerable<PostedTransactionHeader> transactions)
        {
            _invoiceHeaderRepository.SetDBContext(_dbContext);
            _invoiceLineRepository.SetDBContext(_dbContext);
            _postedTransactionHeaderRepository.SetDBContext(_dbContext);

            InvoiceHeader invoiceHeader = new InvoiceHeader
            {
                Status = InvoiceStatus.Invoiced,
                Customer = customer,
                PaymentMethod = paymentMethod,
                Period = period,
            };

            IEnumerable<InvoiceLine> readyInvoiceLines = AggregateLines(transactions);

            if (readyInvoiceLines.Count() <= 0)
            {
                return null;
            }

            try
            {
                _dbContext.BeginDbTransaction();

                invoiceHeader = await _invoiceHeaderRepository.CreateInvoiceHeaderAsync(invoiceHeader);

                List<InvoiceLine> invoiceLines = new List<InvoiceLine>();
                foreach (InvoiceLine line in readyInvoiceLines)
                {
                    InvoiceLine invoiceLine = await _invoiceLineRepository.CreateInvoiceLineAsync(invoiceHeader.Id, line);

                    invoiceLines.Add(invoiceLine);
                }

                invoiceHeader.Lines = invoiceLines;

                foreach (PostedTransactionHeader transactionHeader in transactions)
                {
                    transactionHeader.Status = TransactionStatus.Invoiced;
                    transactionHeader.InvoiceId = invoiceHeader.Id;
                    await _postedTransactionHeaderRepository.UpdatePostedTransactionHeaderAsync(transactionHeader);
                }

                _dbContext.CommitDbTransaction();

                return invoiceHeader;
            }
            catch (Exception ex)
            {
                _dbContext.RollbackDbTransaction();
                throw;
            }
        }

        private IEnumerable<InvoiceLine> AggregateLines(IEnumerable<PostedTransactionHeader> transactions)
        {
            List<InvoiceLine> lines = new List<InvoiceLine>();

            foreach (PostedTransactionHeader transaction in transactions)
            {
                foreach (PostedTransactionLine line in transaction.Lines)
                {
                    InvoiceLine? invoiceLine = lines.Find((x) => x.Product.Id == line.Product.Id);

                    if (invoiceLine == null)
                    {
                        invoiceLine = new InvoiceLine
                        {
                            Product = line.Product,
                            Quantity = 0,
                            Amount = 0
                        };

                        lines.Add(invoiceLine);
                    }

                    invoiceLine.Quantity += line.Quantity;
                    invoiceLine.Amount += line.Amount;
                }
            }

            return lines;
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

        public async Task<InvoiceHeader> MarkAsPaid(long invoiceId)
        {
            _invoiceHeaderRepository.SetDBContext(_dbContext);
            _postedTransactionHeaderRepository.SetDBContext(_dbContext);

            try
            {
                _dbContext.BeginDbTransaction();
                InvoiceHeader invoice = await _invoiceHeaderRepository.GetInvoiceHeaderAsync(invoiceId);
                invoice.Status = InvoiceStatus.Paid;
                invoice = await _invoiceHeaderRepository.UpdateInvoiceHeaderAsync(invoice);

                IEnumerable<PostedTransactionHeader> invoicedTransactions = await _postedTransactionHeaderRepository.GetPostedTransactionHeadersForInvoiceAsync(invoiceId);

                foreach (PostedTransactionHeader transactionHeader in invoicedTransactions)
                {
                    transactionHeader.Status = TransactionStatus.Paid;
                    await _postedTransactionHeaderRepository.UpdatePostedTransactionHeaderAsync(transactionHeader);
                }

                _dbContext.CommitDbTransaction();

                return invoice;
            }
            catch (Exception ex)
            {
                _dbContext.RollbackDbTransaction();
                throw;
            }
        }

        public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInvoiceGeneration(Customer customer, PaymentMethod paymentMethod, Period period)
        {
            bool ret = true;
            List<ValidationError> validationErrors = new List<ValidationError>();

            bool exists = await _customerRepository.CustomerExistsAsync(customer.Id);

            if (!exists)
            {
                validationErrors.Add(new ValidationError { Error = $"Customer with Id {customer.Id} was not found" });
                ret = false;
            }

            exists = await _paymentMethodRepository.PaymentMethodExistsAsync(paymentMethod.Id);

            if (!exists)
            {
                validationErrors.Add(new ValidationError { Error = $"Payment method with Id {paymentMethod.Id} was not found" });
                ret = false;
            }

            if ((period.DateTo - period.DateFrom).TotalDays > 31)
            {
                validationErrors.Add(new ValidationError { Error = $"Period may be at most 31 days." });
                ret = false;
            }

            return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
        }
    }
}
