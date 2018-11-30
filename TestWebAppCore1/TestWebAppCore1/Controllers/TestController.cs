using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestWebAppCore1.Models;

namespace TestWebAppCore1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        Random r = new Random();
        private readonly ILogger _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        // POST api/values
        [HttpPost]
        public async Task<string> PostAsync([FromBody] Payment p)
        {
            var response = new PaymentCallbackResponse();
            response.CorrelationGuid = p.CorrelationGuid;
            response.IsSuccess = r.Next(0, 100) > 30;
            response.Amount = p.Amount;

            if (!response.IsSuccess)
            {
                response.ErrorMessage = "Not succeeded.";
            }

            TestEventHandler.InvokeReceived(response);
            _logger.LogInformation($"{p.CorrelationGuid} event handler invoked");

            return "Ok";
        }

    }
}