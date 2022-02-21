using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Interface
{
    public interface IPaymentService
    {
        MakePaymentResult MakePayment(MakePaymentRequest request);
    }
}
