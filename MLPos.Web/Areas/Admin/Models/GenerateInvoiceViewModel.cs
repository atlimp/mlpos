using MLPos.Core.Model;

namespace MLPos.Web.Models
{
    public class GenerateInvoiceViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public long? SelectedCustomerId { get; set; }
        public IEnumerable<PaymentMethod> PaymentMethods { get; set; }
        public long? SelectedPaymentMethodId { get; set; }
        public Period Period { get; set; }
    }
}
