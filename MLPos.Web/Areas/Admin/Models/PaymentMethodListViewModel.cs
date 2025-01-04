using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class PaymentMethodListViewModel
{
    public IEnumerable<PaymentMethod> PaymentMethods { get; set; }
}