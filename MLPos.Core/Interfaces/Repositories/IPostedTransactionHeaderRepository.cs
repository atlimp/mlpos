using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IPostedTransactionHeaderRepository
    {
        public Task<PostedTransactionHeader> CreatePostedTransactionHeaderAsync(DbTransaction transaction, PostedTransactionHeader transactionHeader);
    }
}
