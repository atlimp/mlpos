using MLPos.Core.Model;

namespace MLPos.Core.Interfaces.Services;

public interface IPaymentMethodService
{
    public Task<PaymentMethod> GetPaymentMethodAsync(long id);
    public Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
    public Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod paymentMethod);
    public Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod);
    public Task DeletePaymentMethodAsync(long id);
    public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(PaymentMethod paymentMethod);
    public Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(PaymentMethod paymentMethod);
}