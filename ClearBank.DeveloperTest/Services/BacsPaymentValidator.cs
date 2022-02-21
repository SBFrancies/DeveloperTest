using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Types;
using System;

namespace ClearBank.DeveloperTest.Services
{
    public class BacsPaymentValidator : IPaymentValidator
    {
        public PaymentScheme PaymentScheme => PaymentScheme.Bacs;

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

            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }
    }
}
