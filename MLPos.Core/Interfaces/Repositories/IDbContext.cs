using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IDbContext : IDisposable
    {
        public DbConnection Connection { get; }
        public DbTransaction Transaction { get; }
        public DbTransaction BeginDbTransaction();
        public void CommitDbTransaction();
        public void RollbackDbTransaction();
    }
}
