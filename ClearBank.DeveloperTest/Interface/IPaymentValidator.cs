using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Interface
{
    public interface IPaymentValidator
    {
        PaymentScheme PaymentScheme { get; }

        bool ValidatePayment(Account account, MakePaymentRequest request);
    }
}
