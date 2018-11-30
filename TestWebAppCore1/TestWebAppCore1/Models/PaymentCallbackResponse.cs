using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebAppCore1.Models
{
    public class PaymentCallbackResponse
    {
        public Guid CorrelationGuid { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int CallbackMiliSeconds { get; set; }
        public int CallsCount { get; set; }
        public double Amount { get; set; }
    }
}
