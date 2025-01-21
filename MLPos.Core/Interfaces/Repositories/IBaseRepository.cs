using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPos.Core.Interfaces.Repositories
{
    public interface IBaseRepository
    {
        public void SetDBContext(IDbContext dbContext);
    }
}
