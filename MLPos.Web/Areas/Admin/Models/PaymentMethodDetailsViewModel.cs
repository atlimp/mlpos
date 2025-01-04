using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class PaymentMethodDetailsViewModel
{
    public bool Editing { get; set; }
    public bool NewPaymentMethod { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public IEnumerable<ValidationError> ValidationErrors { get; set; }
}