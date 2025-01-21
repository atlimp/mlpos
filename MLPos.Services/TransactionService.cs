using MLPos.Core.Enums;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionHeaderRepository _headerRepository;
        private readonly ITransactionLineRepository _lineRepository;
        private readonly IPostedTransactionHeaderRepository _postedTransactionHeaderRepository;
        private readonly IPostedTransactionLineRepository _postedTransactionLineRepository;

        private readonly IPosClientRepository _posClientRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        private readonly IDBAccessor _dbAccessor;

        public TransactionService(ITransactionHeaderRepository headerRepository,
            ITransactionLineRepository lineRepository,
            IPostedTransactionHeaderRepository postedTransactionHeaderRepository,
            IPostedTransactionLineRepository postedTransactionLineRepository,
            IPosClientRepository posClientRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IDBAccessor dbAccessor)
        {
            _headerRepository = headerRepository;
            _lineRepository = lineRepository;
            _postedTransactionHeaderRepository = postedTransactionHeaderRepository;
            _postedTransactionLineRepository = postedTransactionLineRepository;
            _posClientRepository = posClientRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _dbAccessor = dbAccessor;
        }

        public async Task<TransactionHeader?> GetTransactionHeaderAsync(long transactionId, long posClientId)
        {
            TransactionHeader? transactionHeader = await _headerRepository.GetTransactionHeaderAsync(transactionId, posClientId);

            if (transactionHeader == null || transactionHeader.Customer == null)
            {
                return null;
            }

            transactionHeader.Customer = await _customerRepository.GetCustomerAsync(transactionHeader.Customer.Id);

            transactionHeader.Lines = await _lineRepository.GetAllTransactionLinesAsync(transactionId, posClientId);

            foreach (TransactionLine line in transactionHeader.Lines)
            {
                if (line.Product != null)
                    line.Product = await _productRepository.GetProductAsync(line.Product.Id);
            }

            return transactionHeader;
        }

        public async Task<TransactionHeader?> AddItemAsync(TransactionHeader transactionHeader, Product product, decimal qty)
        {
            ThrowIf.Null(transactionHeader);
            ThrowIf.Null(product);

            TransactionLine newLine = new TransactionLine()
            {
                Product = product
            };

            newLine.Quantity = qty;
            newLine.Amount = newLine.Quantity * product.Price;

            await _lineRepository.CreateTransactionLineAsync(transactionHeader.Id, transactionHeader.PosClientId, newLine);

            return await GetTransactionHeaderAsync(transactionHeader.Id, transactionHeader.PosClientId);
        }

        public async Task<TransactionHeader?> CreateTransactionAsync(long posClientId, Customer customer)
        {
            ThrowIf.Null(customer);

            PosClient client = await _posClientRepository.GetPosClientAsync(posClientId);

            TransactionHeader? header = await _headerRepository.CreateTransactionHeaderAsync(new TransactionHeader { Customer = customer, PosClientId = posClientId });

            if (header != null)
            {
                return await GetTransactionHeaderAsync(header.Id, header.PosClientId);
            }

            return null;
        }

        public async Task DeleteTransactionAsync(long id, long posClientId)
        {
            TransactionHeader transactionHeader = await _headerRepository.GetTransactionHeaderAsync(id, posClientId);

            DbTransaction dbTransaction = _dbAccessor.CreateDbTransaction();
            try
            {

                await _postedTransactionHeaderRepository.CreatePostedTransactionHeaderAsync(dbTransaction, this.CreateFrom(transactionHeader, null));
                await _headerRepository.DeleteTransactionHeaderAsync(dbTransaction, id, posClientId);

                _dbAccessor.CommitDbTransaction(dbTransaction);
            }
            catch (Exception ex)
            {
                _dbAccessor.RollbackDbTransaction(dbTransaction);
                throw;
            }
        }

        public async Task<PostedTransactionHeader> PostTransactionAsync(TransactionHeader transactionHeader, PaymentMethod paymentMethod)
        {
            
            DbTransaction dbTransaction = _dbAccessor.CreateDbTransaction();
            try
            {
                PostedTransactionHeader postedTransactionHeader = await _postedTransactionHeaderRepository.CreatePostedTransactionHeaderAsync(dbTransaction, this.CreateFrom(transactionHeader, paymentMethod));
                foreach (PostedTransactionLine line in this.CreateFrom(transactionHeader.Lines))
                {
                    await _postedTransactionLineRepository.CreatePostedTransactionLineAsync(dbTransaction, transactionHeader.Id, transactionHeader.PosClientId, line);
                }

                await _headerRepository.DeleteTransactionHeaderAsync(dbTransaction, transactionHeader.Id, transactionHeader.PosClientId);
                _dbAccessor.CommitDbTransaction(dbTransaction);
                return postedTransactionHeader;
            }
            catch (Exception ex)
            {
                _dbAccessor.RollbackDbTransaction(dbTransaction);
                throw;
            }
        }

        public async Task<TransactionHeader?> RemoveItemAsync(long transactionId, long posClientId, long lineId)
        {
            await _lineRepository.DeleteTransactionLineAsync(transactionId, posClientId, lineId);
            return await GetTransactionHeaderAsync(transactionId, posClientId);
        }

        public async Task<IEnumerable<TransactionHeader>> GetActiveTransactionsAsync(long posClientId)
        {
            var transactionHeaders = await _headerRepository.GetAllTransactionHeaderAsync(posClientId);

            List<TransactionHeader> activeTransactions = new List<TransactionHeader>();
            foreach (var transactionHeader in transactionHeaders)
            {
                activeTransactions.Add(await GetTransactionHeaderAsync(transactionHeader.Id, transactionHeader.PosClientId));
            }

            return activeTransactions;
        }

        public async Task<TransactionSummary> GetTransactionSummaryAsync(long transactionId, long posClientId)
        {
            return await _headerRepository.GetTransactionSummaryAsync(transactionId, posClientId);
        }

        public async Task<IEnumerable<TransactionSummary>> GetAllTransactionSummaryAsync(long posClientId)
        {
            return await _headerRepository.GetAllTransactionSummaryAsync(posClientId);
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
