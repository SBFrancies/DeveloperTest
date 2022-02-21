using AutoFixture;
using ClearBank.DeveloperTest.Controllers;
using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Types;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> _mockPaymentService = new Mock<IPaymentService>();
        private Fixture _fixture = new Fixture();

        [Fact]
        public void PaymentController_Post_SuccessfulPaymentReturnsOk()
        {
            var request = _fixture.Create<MakePaymentRequest>();
            _mockPaymentService.Setup(a => a.MakePayment(request)).Returns(MakePaymentResult.Successful());

            var sut = CreateSystemUnderTest();

            var result = sut.Post(request);

            Assert.IsType<OkResult>(result);
            _mockPaymentService.Verify(a => a.MakePayment(request), Times.Once);
        }

        [Fact]
        public void PaymentController_Post_FailedPaymentReturnsBadRequest()
        {
            var request = _fixture.Create<MakePaymentRequest>();
            _mockPaymentService.Setup(a => a.MakePayment(request)).Returns(MakePaymentResult.Failed());

            var sut = CreateSystemUnderTest();

            var result = sut.Post(request);

            Assert.IsType<BadRequestResult>(result);
            _mockPaymentService.Verify(a => a.MakePayment(request), Times.Once);
        }

        [Fact]
        public void PaymentController_Post_ExceptionReturns500()
        {
            var request = _fixture.Create<MakePaymentRequest>();
            _mockPaymentService.Setup(a => a.MakePayment(request)).Throws(new Exception("test"));

            var sut = CreateSystemUnderTest();

            var result = sut.Post(request);

            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = (ObjectResult)result;

            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("test", statusCodeResult.Value);
            _mockPaymentService.Verify(a => a.MakePayment(request), Times.Once);
        }

        private PaymentController CreateSystemUnderTest()
        {
            return new PaymentController(_mockPaymentService.Object);
        }
    }
}
