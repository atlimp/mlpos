using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionHeaderRepository _headerRepository;
        private readonly ITransactionLineRepository _lineRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public TransactionService(ITransactionHeaderRepository headerRepository, ITransactionLineRepository lineRepository, ICustomerRepository customerRepository, IProductRepository productRepository)
        {
            _headerRepository = headerRepository;
            _lineRepository = lineRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<TransactionHeader?> GetTransactionHeaderAsync(long transactionId)
        {
            TransactionHeader? transactionHeader = await _headerRepository.GetTransactionHeaderAsync(transactionId);

            if (transactionHeader == null || transactionHeader.Customer == null)
            {
                return null;
            }

            transactionHeader.Customer = await _customerRepository.GetCustomerAsync(transactionHeader.Customer.Id);

            transactionHeader.Lines = await _lineRepository.GetAllTransactionLinesAsync(transactionId);

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

            await _lineRepository.CreateTransactionLineAsync(transactionHeader.Id, newLine);

            return await GetTransactionHeaderAsync(transactionHeader.Id);
        }

        public async Task<TransactionHeader?> CreateTransactionAsync(Customer customer)
        {
            ThrowIf.Null(customer);
            ThrowIf.Null(customer);

            TransactionHeader? header = await _headerRepository.CreateTransactionHeaderAsync(new TransactionHeader { Customer = customer });

            if (header != null)
            {
                return await GetTransactionHeaderAsync(header.Id);
            }

            return null;
        }

        public async Task DeleteTransactionAsync(long id)
        {
            await _headerRepository.DeleteTransactionHeaderAsync(id);
        }

        public Task PostTransactionAsync(TransactionHeader transactionHeader, PaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionHeader?> RemoveItemAsync(long transactionId, long lineId)
        {
            await _lineRepository.DeleteTransactionLineAsync(transactionId, lineId);
            return await GetTransactionHeaderAsync(transactionId);
        }

        public async Task<IEnumerable<TransactionHeader>> GetActiveTransactionsAsync()
        {
            var transactionHeaders = await _headerRepository.GetAllTransactionHeaderAsync();

            List<TransactionHeader> activeTransactions = new List<TransactionHeader>();
            foreach (var transactionHeader in transactionHeaders)
            {
                activeTransactions.Add(await GetTransactionHeaderAsync(transactionHeader.Id));
            }

            return activeTransactions;
        }
    }
}
