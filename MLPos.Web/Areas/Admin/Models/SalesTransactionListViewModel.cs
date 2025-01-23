using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class SalesTransactionListViewModel
{
    public IEnumerable<PostedTransactionHeader> Transactions { get; set; }
    public int PageNum { get; set; }
    public bool HasMorePages { get; set; }
}