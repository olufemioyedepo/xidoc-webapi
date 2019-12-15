using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Enums
{
    public class PaymentMethod
    {
        enum PaymentMethods
        {
            Non,
            Cash,
            Cheque,
            Transfer,
            USSD
        }
    }
}
