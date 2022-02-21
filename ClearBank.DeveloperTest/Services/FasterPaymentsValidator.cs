using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Types;
using System;

namespace ClearBank.DeveloperTest.Services
{
    public class FasterPaymentsValidator : IPaymentValidator
    {
        public PaymentScheme PaymentScheme => PaymentScheme.FasterPayments;

        public bool ValidatePayment(Account account, MakePaymentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) || account.Balance < request.Amount)
            {
                return false;
            }

            return true;

        }
    }
}
