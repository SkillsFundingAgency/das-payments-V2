using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration
{
    public class ServiceConfig : IServiceConfig
    {
        public string StorageConnectionString { get; set; }
        public string IncomingEndpointName { get; set; }
        public string OutgoingEndpointName { get; set; }
        public string DestinationEndpointName { get; set; }
        public string LoggerConnectionstring { get; set; }
    }
}
