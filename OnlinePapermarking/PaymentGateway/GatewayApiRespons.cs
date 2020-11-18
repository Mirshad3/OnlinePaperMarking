using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePapermarking.PaymentGateway
{
    public class GatewayApiRespons
    {
        public String sessionID { get; set; }
        //public String TransactionId { get; set; }

        public String ApiMethod { get; set; } = "PUT";
        public String RequestUrl { get; set; }
        public String Payload { get; set; }
        public String Password { get; set; }
        public String ApiUsername { get; set; }
        public String Merchant { get; set; }
        public String OrderId { get; set; }
        public String ApiOperation { get; set; }
    }
}