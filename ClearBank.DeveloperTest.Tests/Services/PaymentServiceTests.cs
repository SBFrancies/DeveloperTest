using AutoFixture;
using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<IDataStore> _mockDataStore = new Mock<IDataStore>();
        private readonly Mock<IPaymentValidator> _mockValidator1 = new Mock<IPaymentValidator>();
        private readonly Mock<IPaymentValidator> _mockValidator2 = new Mock<IPaymentValidator>();
        private readonly Mock<IPaymentValidator> _mockValidator3 = new Mock<IPaymentValidator>();
        private readonly Mock<ILogger<PaymentService>> _mockLogger = new Mock<ILogger<PaymentService>>();
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void PaymentService_MakePayment_AccountNotFoundReturnsFalse()
        {
            var request = _fixture.Create<MakePaymentRequest>();
            _mockDataStore.Setup(a => a.GetAccount(request.DebtorAccountNumber)).Returns((Account)null);
            var sut = CreateSystemUnderTest();

            var result = sut.MakePayment(request);

            Assert.False(result.Success);
            _mockDataStore.Verify(a => a.GetAccount(request.DebtorAccountNumber), Times.Once);
            _mockValidator1.Verify(a => a.ValidatePayment(It.IsAny<Account>(), request), Times.Never);
            _mockValidator2.Verify(a => a.ValidatePayment(It.IsAny<Account>(), request), Times.Never);
            _mockValidator3.Verify(a => a.ValidatePayment(It.IsAny<Account>(), request), Times.Never);
            _mockLogger.VerifyLog(LogLevel.Warning, $"No account found for account number {request.DebtorAccountNumber}", null, Times.Once());
        }

        [Fact]
        public void PaymentService_MakePayment_ValidatorNotFoundThrowsException()
        {
            var request = _fixture.Build<MakePaymentRequest>().With(a => a.PaymentScheme, PaymentScheme.FasterPayments).Create();
            var account = _fixture.Create<Account>();
            _mockDataStore.Setup(a => a.GetAccount(request.DebtorAccountNumber)).Returns(account);
            _mockValidator1.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.Bacs);
            _mockValidator2.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.Bacs);
            _mockValidator3.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.Bacs);
            var sut = CreateSystemUnderTest();

            Assert.Throws<Exception>(() => sut.MakePayment(request));

            _mockDataStore.Verify(a => a.GetAccount(request.DebtorAccountNumber), Times.Once);
            _mockValidator1.Verify(a => a.ValidatePayment(account, request), Times.Never);
            _mockValidator2.Verify(a => a.ValidatePayment(account, request), Times.Never);
            _mockValidator3.Verify(a => a.ValidatePayment(account, request), Times.Never);
            _mockLogger.VerifyLog(LogLevel.Warning, $"No account found for account number {request.DebtorAccountNumber}", null, Times.Never());
            _mockLogger.VerifyLog(LogLevel.Error, $"No validator found for payment scheme {request.PaymentScheme}", null, Times.Once());
        }

        [Fact]
        public void PaymentService_MakePayment_PayemntFailsValidationReturnFalse()
        {
            var request = _fixture.Build<MakePaymentRequest>().With(a => a.PaymentScheme, PaymentScheme.FasterPayments).Create();
            var account = _fixture.Create<Account>();
            var intialBalance = account.Balance;
            _mockDataStore.Setup(a => a.GetAccount(request.DebtorAccountNumber)).Returns(account);
            _mockValidator1.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.FasterPayments);
            _mockValidator1.Setup(a => a.ValidatePayment(account, request)).Returns(false);
            _mockValidator2.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.Bacs);
            _mockValidator3.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.Bacs);
            var sut = CreateSystemUnderTest();

            var result = sut.MakePayment(request);
            Assert.False(result.Success);
            Assert.Equal(intialBalance, account.Balance);

            _mockDataStore.Verify(a => a.GetAccount(request.DebtorAccountNumber), Times.Once);
            _mockValidator1.Verify(a => a.ValidatePayment(account, request), Times.Once);
            _mockValidator2.Verify(a => a.ValidatePayment(account, request), Times.Never);
            _mockValidator3.Verify(a => a.ValidatePayment(account, request), Times.Never);
            _mockLogger.VerifyLog(LogLevel.Warning, $"No account found for account number {request.DebtorAccountNumber}", null, Times.Never());
            _mockLogger.VerifyLog(LogLevel.Error, $"No validator found for payment scheme {request.PaymentScheme}", null, Times.Never());
            _mockLogger.VerifyLog(LogLevel.Warning, "Payment was not successful", null, Times.Once());
            _mockDataStore.Verify(a => a.UpdateAccount(account), Times.Never);
        }

        [Fact]
        public void PaymentService_MakePayment_PayemntPassesValidationReturnTrue()
        {
            var request = _fixture.Build<MakePaymentRequest>().With(a => a.PaymentScheme, PaymentScheme.FasterPayments).Create();
            var account = _fixture.Create<Account>();
            var intialBalance = account.Balance;
            _mockDataStore.Setup(a => a.GetAccount(request.DebtorAccountNumber)).Returns(account);
            _mockValidator1.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.FasterPayments);
            _mockValidator1.Setup(a => a.ValidatePayment(account, request)).Returns(true);
            _mockValidator2.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.Bacs);
            _mockValidator3.SetupGet(a => a.PaymentScheme).Returns(PaymentScheme.Bacs);
            var sut = CreateSystemUnderTest();

            var result = sut.MakePayment(request);
            Assert.True(result.Success);
            Assert.Equal(intialBalance - request.Amount, account.Balance);

            _mockDataStore.Verify(a => a.GetAccount(request.DebtorAccountNumber), Times.Once);
            _mockValidator1.Verify(a => a.ValidatePayment(It.IsAny<Account>(), request), Times.Once);
            _mockValidator2.Verify(a => a.ValidatePayment(It.IsAny<Account>(), request), Times.Never);
            _mockValidator3.Verify(a => a.ValidatePayment(It.IsAny<Account>(), request), Times.Never);
            _mockLogger.VerifyLog(LogLevel.Warning, $"No account found for account number {request.DebtorAccountNumber}", null, Times.Never());
            _mockLogger.VerifyLog(LogLevel.Error, $"No validator found for payment scheme {request.PaymentScheme}", null, Times.Never());
            _mockLogger.VerifyLog(LogLevel.Warning, "Payment was not successful", null, Times.Never());
            _mockLogger.VerifyLog(LogLevel.Information, "Payment successful, account updated", null, Times.Once());
            _mockDataStore.Verify(a => a.UpdateAccount(account), Times.Once);
        }

        private PaymentService CreateSystemUnderTest()
        {
            return new PaymentService(_mockDataStore.Object, new[] { _mockValidator1.Object, _mockValidator2.Object, _mockValidator3.Object }, _mockLogger.Object);
        }
    }
};