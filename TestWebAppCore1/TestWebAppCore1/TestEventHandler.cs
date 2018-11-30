using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebAppCore1.Models;

namespace TestWebAppCore1
{
    public delegate void TestDelegateEventHandler(PaymentCallbackResponse p);

    public static class TestEventHandler
    {        
        public static event TestDelegateEventHandler Received;        

        public static void InvokeReceived(PaymentCallbackResponse p)
        {
            var handler = Received;
            if (handler != null)
                handler(p);
        }
    }
}
