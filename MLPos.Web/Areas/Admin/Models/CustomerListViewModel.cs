using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class CustomerListViewModel
{
    public IEnumerable<Customer> Customers { get; set; }
}