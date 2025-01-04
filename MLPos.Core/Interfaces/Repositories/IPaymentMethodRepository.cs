using MLPos.Core.Model;

namespace MLPos.Core.Interfaces.Repositories;

public interface IPaymentMethodRepository
{
    public Task<PaymentMethod> GetPaymentMethodAsync(long id);
    public Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
    public Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod paymentMethod);
    public Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod);
    public Task DeletePaymentMethodAsync(long id);
    public Task<bool> PaymentMethodExistsAsync(long id);
}