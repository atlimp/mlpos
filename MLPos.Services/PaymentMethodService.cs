using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;

namespace MLPos.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }
    
    public async Task<PaymentMethod> GetPaymentMethodAsync(long id)
    {
        return await _paymentMethodRepository.GetPaymentMethodAsync(id);
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
    {
        return await _paymentMethodRepository.GetPaymentMethodsAsync();
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod paymentMethod)
    {
        if (paymentMethod.Image == null)
        {
            paymentMethod.Image = string.Empty;
        }

        return await _paymentMethodRepository.CreatePaymentMethodAsync(paymentMethod);
    }

    public async Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
    {
        if (paymentMethod.Image == null)
        {
            paymentMethod.Image = string.Empty;
        }

        return await _paymentMethodRepository.UpdatePaymentMethodAsync(paymentMethod);
    }

    public async Task DeletePaymentMethodAsync(long id)
    {
        await _paymentMethodRepository.DeletePaymentMethodAsync(id);
    }

    public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateUpdate(PaymentMethod paymentMethod)
    {
        bool ret = true;
        List<ValidationError> validationErrors = new List<ValidationError>();
        
        bool exists = await _paymentMethodRepository.PaymentMethodExistsAsync(paymentMethod.Id);
        
        if (!exists)
        {
            validationErrors.Add(new ValidationError{ Error = $"PaymentMethod with Id {paymentMethod.Id} does not exist!" });
            ret = false;
        }
        
        return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
    }

    public async Task<Tuple<bool, IEnumerable<ValidationError>>> ValidateInsert(PaymentMethod paymentMethod)
    {
        bool ret = true;
        List<ValidationError> validationErrors = new List<ValidationError>();
        
        bool exists = await _paymentMethodRepository.PaymentMethodExistsAsync(paymentMethod.Id);
        
        if (exists)
        {
            validationErrors.Add(new ValidationError{ Error = $"PaymentMethod with Id {paymentMethod.Id} already exists!" });
            ret = false;
        }

        return new Tuple<bool, IEnumerable<ValidationError>>(ret, validationErrors);
    }
}