using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IPostedTransactionLineRepository : IBaseRepository
    {
        public Task<PostedTransactionLine> CreatePostedTransactionLineAsync(long transactionId, long posClientId, PostedTransactionLine line);
    }
}
