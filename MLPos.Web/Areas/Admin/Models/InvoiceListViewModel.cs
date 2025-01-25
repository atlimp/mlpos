using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class InvoiceListViewModel
{
    public IEnumerable<InvoiceHeader> Invoices { get; set; }
    public int PageNum { get; set; }
    public bool HasMorePages { get; set; }
}