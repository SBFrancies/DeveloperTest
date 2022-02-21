using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ClearBank.DeveloperTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IPaymentService PaymentService { get; }

        public PaymentController(IPaymentService paymentService)
        {
            PaymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult Post([FromBody]MakePaymentRequest request)
        {
            try
            {
                var result = PaymentService.MakePayment(request);

                if(result.Success)
                {
                    return Ok();
                }

                else
                {
                    return BadRequest();
                }
            }

            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.InnerException?.Message ?? exception.Message);
            }
        }
    }
}
