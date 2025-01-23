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

        private readonly IDbContext _dbContext;

        public TransactionPostingService(ITransactionHeaderRepository headerRepository,
            IPostedTransactionHeaderRepository postedTransactionHeaderRepository,
            IPostedTransactionLineRepository postedTransactionLineRepository,
            IDbContext dbContext)
        {
            _headerRepository = headerRepository;
            _postedTransactionHeaderRepository = postedTransactionHeaderRepository;
            _postedTransactionLineRepository = postedTransactionLineRepository;
            _dbContext = dbContext;
        }

        public async Task<PostedTransactionHeader> PostTransactionAsync(TransactionHeader transactionHeader, PaymentMethod paymentMethod)
        {
            PostedTransactionHeader postedTransaction = await PostTransaction(transactionHeader, paymentMethod);

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
                foreach (PostedTransactionLine line in this.CreateFrom(transactionHeader.Lines))
                {
                    await _postedTransactionLineRepository.CreatePostedTransactionLineAsync(transactionHeader.Id, transactionHeader.PosClientId, line);
                }

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
