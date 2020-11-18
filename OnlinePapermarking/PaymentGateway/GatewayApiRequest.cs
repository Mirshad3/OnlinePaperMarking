using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePapermarking.PaymentGateway
{
    public class GatewayApiRequest
    {
        //private GatewayApiConfig gatewayApiConfig;
        public String OrderId { get; set; }
        //public String TransactionId { get; set; }

        
        public String ApiOperation { get; set; }
        public String ApiMethod { get; set; } = "PUT";
        public String RequestUrl { get; set; }
        public String Payload { get; set; }

        //public String SessionId { get; set; }
        //public String SecureId { get; set; }
        //public String SecureIdResponseUrl { get; set; }

        //public String SourceType { get; set; }
        //public String CardNumber { get; set; }
        //public String ExpiryMonth { get; set; }
        //public String ExpiryYear { get; set; }
        //public String SecurityCode { get; set; }
        
        public String OrderAmount { get; set; }
        
        public String OrderCurrency { get; set; }
        
        public String OrderDescription { get; set; }

        //public String TransactionAmount { get; set; }
        //public String TransactionCurrency { get; set; }
        //public String TargetTransactionId { get; set; }
        
        public String ReturnUrl { get; set; }

        //public String PaymentAuthResponse { get; set; }
        //public String SecureId3D { get; set; }


        //public String BrowserPaymentOperation { get; set; }
        //public String BrowserPaymentPaymentConfirmation { get; set; }


        //public Dictionary<String, String> NVPParameters { get; set; }

        //public String ContentType { get; set; } = "application/json; charset=iso-8859-1";


        //public String MasterpassOnline { get; set; }
        //public String MasterpassOriginUrl { get; set; }

        //public String MasterpassOauthToken { get; set; }
        //public String MasterpassOauthVerifier { get; set; }
        //public String MasterpassCheckoutUrl { get; set; }


        //public String Token { get; set; }

        //private String apiBaseUrl { get; set; }
        
        public String Password { get; set; }
        
        public String ApiUsername { get; set; }
        
        public String Merchant { get; set; }
        
        public String InteractionOperation { get; set; }
        
        public String InteractionMerchantName { get; set; }
        

    }
}