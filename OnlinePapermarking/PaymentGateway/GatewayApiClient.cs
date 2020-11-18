using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePapermarking.PaymentGateway
{
    public class GatewayApiClient : AbstractGatewayApiClient
    {
        //private readonly GatewayApiConfig GatewayApiConfig;

        //public GatewayApiClient(IOptions<GatewayApiConfig> gatewayApiConfig, ILogger<GatewayApiClient> logger)
        //{
        //    GatewayApiConfig = gatewayApiConfig.Value;
        //    this.Logger = logger;
        //}

        /// <summary>
        /// Sends the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        public override string SendTransaction(GatewayApiRequest gatewayApiRequest)
        {
            return this.executeHTTPMethod(gatewayApiRequest);
        }

    }
}