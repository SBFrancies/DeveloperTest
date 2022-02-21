using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private IDataStore DataStore { get; }
        private IEnumerable<IPaymentValidator> PaymentVaidators { get; }
        private ILogger<PaymentService> Logger { get; }

        public PaymentService(IDataStore dataStore, IEnumerable<IPaymentValidator> paymentVaidators, ILogger<PaymentService> logger)
        {
            DataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            PaymentVaidators = paymentVaidators ?? throw new ArgumentNullException(nameof(paymentVaidators));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var account = DataStore.GetAccount(request.DebtorAccountNumber);

            if(account == null)
            {
                Logger.LogWarning($"No account found for account number {request.DebtorAccountNumber}");
                return MakePaymentResult.Failed();
            }

            var validator = PaymentVaidators.FirstOrDefault(a => a.PaymentScheme == request.PaymentScheme);

            if(validator == null)
            {
                Logger.LogError($"No validator found for payment scheme {request.PaymentScheme}");
                throw new Exception($"No validator found for payment scheme {request.PaymentScheme}");
            }

            var success = validator.ValidatePayment(account, request);

            if(success)
            {
                account.DeductFromBalance(request.Amount);
                DataStore.UpdateAccount(account);
                Logger.LogInformation("Payment successful, account updated");
            }

            else
            {
                Logger.LogWarning("Payment was not successful");
            }

            return success ? MakePaymentResult.Successful() : MakePaymentResult.Failed();
        }
    }
}
