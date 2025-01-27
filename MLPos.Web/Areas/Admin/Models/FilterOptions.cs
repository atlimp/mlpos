using MLPos.Core.Enums;
using MLPos.Core.Model;

namespace MLPos.Web.Models
{
    public class FilterOptions
    {
        public IEnumerable<Customer> Customers { get; set; }
        public long? SelectedCustomerId { get; set; }
        public IEnumerable<PaymentMethod> PaymentMethods { get; set; }
        public long? SelectedPaymentMethodId { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public Dictionary<int, string> StatusValues { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
