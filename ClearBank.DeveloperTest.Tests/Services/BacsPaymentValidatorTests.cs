using AutoFixture;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class BacsPaymentValidatorTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Theory]
        [InlineData(AllowedPaymentSchemes.Bacs, true)]
        [InlineData(AllowedPaymentSchemes.FasterPayments, false)]
        [InlineData(AllowedPaymentSchemes.Chaps, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments, true)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, true)]
        [InlineData(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments, false)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps, true)]
        public void BacsPaymentValidator_ValidatePayment_HasBacsFlagReturnsTrue(AllowedPaymentSchemes schemes, bool success)
        {
            var request = _fixture.Create<MakePaymentRequest>();
            var account = _fixture.Build<Account>().With(a => a.AllowedPaymentSchemes, schemes).Create();
            var sut = CreateSystemUnderTest();

            var result = sut.ValidatePayment(account, request);

            Assert.Equal(success, result);

        }

        private BacsPaymentValidator CreateSystemUnderTest()
        {
            return new BacsPaymentValidator();
        }
    }
}
