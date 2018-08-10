using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration
{
    public class ServiceConfig : IServiceConfig
    {
        public string StorageConnectionString { get; set; }
        public string IncomingEndpointName { get; set; }
        public string OutgoingEndpointName { get; set; }
        public string DestinationEndpointName { get; set; }
    }
}
