using AutoFixture;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class ChapsPaymentValidatorTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Theory]
        [InlineData(AllowedPaymentSchemes.Bacs, AccountStatus.Live, false)]
        [InlineData(AllowedPaymentSchemes.FasterPayments, AccountStatus.Live, false)]
        [InlineData(AllowedPaymentSchemes.Chaps, AccountStatus.Live, true)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments, AccountStatus.Live, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, AccountStatus.Live, true)]
        [InlineData(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments, AccountStatus.Live, true)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps, AccountStatus.Live, true)]
        [InlineData(AllowedPaymentSchemes.Bacs, AccountStatus.Disabled, false)]
        [InlineData(AllowedPaymentSchemes.FasterPayments, AccountStatus.Disabled, false)]
        [InlineData(AllowedPaymentSchemes.Chaps, AccountStatus.Disabled, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments, AccountStatus.Disabled, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, AccountStatus.Disabled, false)]
        [InlineData(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments, AccountStatus.Disabled, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps, AccountStatus.Disabled, false)]
        [InlineData(AllowedPaymentSchemes.Bacs, AccountStatus.InboundPaymentsOnly, false)]
        [InlineData(AllowedPaymentSchemes.FasterPayments, AccountStatus.InboundPaymentsOnly, false)]
        [InlineData(AllowedPaymentSchemes.Chaps, AccountStatus.InboundPaymentsOnly, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments, AccountStatus.InboundPaymentsOnly, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, AccountStatus.InboundPaymentsOnly, false)]
        [InlineData(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments, AccountStatus.InboundPaymentsOnly, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps, AccountStatus.InboundPaymentsOnly, false)]
        public void ChapsPaymentValidator_ValidatePayment_HasChapsFlagAndLiveAccountReturnsfalse(AllowedPaymentSchemes schemes, AccountStatus status, bool success)
        {
            var request = _fixture.Create<MakePaymentRequest>();
            var account = _fixture.Build<Account>().With(a => a.AllowedPaymentSchemes, schemes).With(a => a.Status, status).Create();
            var sut = CreateSystemUnderTest();

            var result = sut.ValidatePayment(account, request);

            Assert.Equal(success, result);
        }

        private ChapsPaymentValidator CreateSystemUnderTest()
        {
            return new ChapsPaymentValidator();
        }
    }
}
