﻿using MLPos.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IPostedTransactionHeaderRepository : IBaseRepository
    {
        public Task<PostedTransactionHeader> CreatePostedTransactionHeaderAsync(PostedTransactionHeader transactionHeader);
        public Task<PostedTransactionHeader> UpdatePostedTransactionHeaderAsync(PostedTransactionHeader transactionHeader);
        public Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersAsync(int limit, int offset);
        public Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersAsync(PostedTransactionQueryFilter filter);
        public Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersAsync(PostedTransactionQueryFilter filter, int limit, int offset);
        public Task<IEnumerable<PostedTransactionHeader>> GetPostedTransactionHeadersForInvoiceAsync(long invoiceId);
        public Task<PostedTransactionHeader> GetPostedTransactionHeaderAsync(long transactionId, long posClientId);
    }
}
