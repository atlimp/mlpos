using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class CustomerDetailsViewModel
{
    public bool Editing { get; set; }
    public bool NewCustomer { get; set; }
    public Customer Customer { get; set; }
    public IEnumerable<ValidationError> ValidationErrors { get; set; }
}