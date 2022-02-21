using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Types;
using System;

namespace ClearBank.DeveloperTest.Services
{
    public class ChapsPaymentValidator : IPaymentValidator
    {
        public PaymentScheme PaymentScheme => PaymentScheme.Chaps;

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

            if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) || account.Status != AccountStatus.Live)
            {
                return false;
            }

            return true;
        }
    }
}
