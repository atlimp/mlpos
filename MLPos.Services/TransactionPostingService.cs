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
    public class TransactionPostingService : ITransactionPostingService
    {

        private readonly ITransactionHeaderRepository _headerRepository;
        private readonly IPostedTransactionHeaderRepository _postedTransactionHeaderRepository;
        private readonly IPostedTransactionLineRepository _postedTransactionLineRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IInvoicingService _invoicingService;

        private readonly IDbContext _dbContext;

        public TransactionPostingService(ITransactionHeaderRepository headerRepository,
            IPostedTransactionHeaderRepository postedTransactionHeaderRepository,
            IPostedTransactionLineRepository postedTransactionLineRepository,
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository,
            ICustomerRepository customerRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IInvoicingService invoicingService,
            IDbContext dbContext)
        {
            _headerRepository = headerRepository;
            _postedTransactionHeaderRepository = postedTransactionHeaderRepository;
            _postedTransactionLineRepository = postedTransactionLineRepository;
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _invoicingService = invoicingService;
            _dbContext = dbContext;
        }

        public async Task<PostedTransactionHeader> PostTransactionAsync(TransactionHeader transactionHeader, PaymentMethod paymentMethod)
        {
            PostedTransactionHeader postedTransaction = await PostTransaction(transactionHeader, paymentMethod);
            await CreateInventoryTransactions(postedTransaction);

            if (postedTransaction.Status == TransactionStatus.Posted && postedTransaction.PaymentMethod.InvoiceOnPost)
            {
                await _invoicingService.GenerateInvoice(postedTransaction);
            }

            return postedTransaction;
        }


        private async Task<PostedTransactionHeader> PostTransaction(TransactionHeader transactionHeader, PaymentMethod paymentMethod)
        {
            _postedTransactionHeaderRepository.SetDBContext(_dbContext);
            _postedTransactionLineRepository.SetDBContext(_dbContext);
            _headerRepository.SetDBContext(_dbContext);
            try
            {
                _dbContext.BeginDbTransaction();

                PostedTransactionHeader postedTransactionHeader = await _postedTransactionHeaderRepository.CreatePostedTransactionHeaderAsync(this.CreateFrom(transactionHeader, paymentMethod));
                postedTransactionHeader.Customer = await _customerRepository.GetCustomerAsync(postedTransactionHeader.Customer.Id);

                if (postedTransactionHeader.PaymentMethod.Id != -1)
                {
                    postedTransactionHeader.PaymentMethod = await _paymentMethodRepository.GetPaymentMethodAsync(postedTransactionHeader.PaymentMethod.Id);
                }

                List<PostedTransactionLine> postedTransactionLines = new List<PostedTransactionLine>();
                foreach (PostedTransactionLine line in this.CreateFrom(transactionHeader.Lines))
                {
                    postedTransactionLines.Add(await _postedTransactionLineRepository.CreatePostedTransactionLineAsync(transactionHeader.Id, transactionHeader.PosClientId, line));
                }

                postedTransactionHeader.Lines = postedTransactionLines;

                await _headerRepository.DeleteTransactionHeaderAsync(transactionHeader.Id, transactionHeader.PosClientId);
                _dbContext.CommitDbTransaction();
                return postedTransactionHeader;
            }
            catch (Exception ex)
            {
                _dbContext.RollbackDbTransaction();
                throw;
            }
        }

        private async Task CreateInventoryTransactions(PostedTransactionHeader transactionHeader)
        {
            foreach (var line in transactionHeader.Lines)
            {
                Product product = await _productRepository.GetProductAsync(line.Product.Id);

                if (product.Type == ProductType.Item)
                {
                    InventoryTransaction inventoryTransaction = new InventoryTransaction();
                    inventoryTransaction.Type = InventoryTransactionType.Sale;
                    inventoryTransaction.TransactionId = transactionHeader.Id;
                    inventoryTransaction.PosClientId = transactionHeader.PosClientId;
                    inventoryTransaction.TransactionLineId = line.Id;
                    inventoryTransaction.ProductId = product.Id;
                    inventoryTransaction.Quantity = line.Quantity;

                    await _inventoryRepository.CreateInventoryTransactionAsync(inventoryTransaction);
                }
            }
        }

        private PostedTransactionHeader CreateFrom(TransactionHeader transactionHeader, PaymentMethod paymentMethod)
        {
            return new PostedTransactionHeader()
            {
                Id = transactionHeader.Id,
                PosClientId = transactionHeader.PosClientId,
                Status = paymentMethod == null ? TransactionStatus.Canceled : TransactionStatus.Posted,
                Customer = transactionHeader.Customer,
                PaymentMethod = paymentMethod,

            };
        }

        private IEnumerable<PostedTransactionLine> CreateFrom(IEnumerable<TransactionLine> lines)
        {
            List<PostedTransactionLine> postedTransactionLines = new List<PostedTransactionLine>();

            foreach (var line in lines)
            {
                PostedTransactionLine found = postedTransactionLines.FirstOrDefault(x => x.Product.Id == line.Product.Id);

                if (found != null)
                {
                    found.Amount += line.Amount;
                    found.Quantity += line.Quantity;
                }
                else
                {
                    postedTransactionLines.Add(CreateFrom(line));
                }
            }

            return postedTransactionLines;
        }

        private PostedTransactionLine CreateFrom(TransactionLine line)
        {
            return new PostedTransactionLine()
            {
                Product = line.Product,
                Amount = line.Amount,
                Quantity = line.Quantity,
            };
        }
    }
}
