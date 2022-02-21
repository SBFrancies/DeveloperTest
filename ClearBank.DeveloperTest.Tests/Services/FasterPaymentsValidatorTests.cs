using AutoFixture;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class FasterPaymentsValidatorTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Theory]
        [InlineData(AllowedPaymentSchemes.Bacs, false)]
        [InlineData(AllowedPaymentSchemes.FasterPayments, true)]
        [InlineData(AllowedPaymentSchemes.Chaps, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments, true)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, false)]
        [InlineData(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments, true)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps, true)]
        public void FasterPaymentsValidator_ValidatePayment_HasFasterPaymentsFlagReturnsTrue(AllowedPaymentSchemes schemes, bool success)
        {
            var request = _fixture.Build<MakePaymentRequest>().With(a => a.Amount, 1).Create();
            var account = _fixture.Build<Account>().With(a => a.AllowedPaymentSchemes, schemes).With(a => a.Balance, 2).Create();
            var sut = CreateSystemUnderTest();

            var result = sut.ValidatePayment(account, request);

            Assert.Equal(success, result);
        }

        [Theory]
        [InlineData(1000, 10, false)]
        [InlineData(10, 10, true)]
        [InlineData(1.23, 23231.2, true)]
        [InlineData(19.8, 10, false)]
        public void FasterPaymentsValidator_ValidatePayment_HasGreaterBalanceFlagReturnsTrue(decimal amount, decimal balance, bool success)
        {
            var request = _fixture.Build<MakePaymentRequest>().With(a => a.Amount, amount).Create();
            var account = _fixture.Build<Account>().With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.FasterPayments).With(a => a.Balance, balance).Create();
            var sut = CreateSystemUnderTest();

            var result = sut.ValidatePayment(account, request);

            Assert.Equal(success, result);
        }

        private FasterPaymentsValidator CreateSystemUnderTest()
        {
            return new FasterPaymentsValidator();
        }
    }
}
