using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TestWebAppCore1.Models;

namespace TestWebAppCore1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private Random r = new Random();
        private bool isCallbackReturned = false;
        private int callsCount = 0;
        private PaymentCallbackResponse _p;
        private Guid _correlationGuid;
        private readonly ILogger _logger;


        public ValuesController(ILogger<ValuesController> logger)
        {
            TestEventHandler.Received += Received;
            _logger = logger;
        }

        private void Received(PaymentCallbackResponse p)
        {
            _logger.LogInformation($"{p.CorrelationGuid} event handler received");
            if (p.CorrelationGuid == _correlationGuid)
            {
                _p = p;
                callsCount = callsCount + 1;
                isCallbackReturned = true;
                _logger.LogInformation($"{p.CorrelationGuid} event handler matched");
            }
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<PaymentCallbackResponse> PostAsync([FromBody] Payment value)
        {
            var tt = await ProcessPostCallAsync(value);
            return tt;
        }      

        private async Task<PaymentCallbackResponse> ProcessPostCallAsync(Payment p)
        {
            //var cts = new CancellationTokenSource();
            //cts.CancelAfter(5000);
            var response = new PaymentCallbackResponse();
            _correlationGuid = p.CorrelationGuid;
            int secondsPassed = 0;

            try
            {
                TestAsync(p);                

                bool isPaymentTimeoutOver = false;
                
                while (!isPaymentTimeoutOver)
                {
                    //Thread.Sleep(4000);           

                    if (isCallbackReturned)
                    {
                        isPaymentTimeoutOver = true;
                        response = _p;
                        response.CallsCount = callsCount;
                        response.CallbackMiliSeconds = secondsPassed;
                        TestEventHandler.Received -= Received;
                    }
                    else
                    {
                        Thread.Sleep(500);
                        secondsPassed = secondsPassed + 500;
                        response.CallbackMiliSeconds = secondsPassed;
                        response.CallsCount = callsCount;

                        if (secondsPassed == p.Seconds)
                        {
                            isPaymentTimeoutOver = true;
                            response.ErrorMessage = "Timeout reached.";
                            TestEventHandler.Received -= Received;
                            _logger.LogInformation($"{p.CorrelationGuid} event handler timeout reached");
                        }
                    }
                    
                }             

                return response;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex.Message);
            }

            return response;
        }

        private async Task TestAsync(Payment p)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(p.CallbackUrl);
                    var result = await client.PostAsJsonAsync("", p); //.AsTask(cts.Token);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    _logger.LogInformation($"{p.CorrelationGuid}: {resultContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
