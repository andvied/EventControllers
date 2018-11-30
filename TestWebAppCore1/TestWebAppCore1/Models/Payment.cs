using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebAppCore1.Models
{
    public class Payment
    {
        public Guid CorrelationGuid { get; set; }
        public double Amount { get; set; }
        public string CallbackUrl { get; set; }
        public int Seconds { get; set; }
    }
}
