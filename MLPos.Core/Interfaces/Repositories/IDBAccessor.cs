using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IDBAccessor
    {
        public DbTransaction CreateDbTransaction();
        public void CommitDbTransaction(DbTransaction transaction);
        public void RollbackDbTransaction(DbTransaction transaction);
    }
}
