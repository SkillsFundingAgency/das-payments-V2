﻿using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;
namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class BindingBootstrapper : BindingsBase
    {
        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<FundingSourcePaymentEvent>());
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(CalculatedRequiredCoInvestedAmount), EndpointNames.NonLevyFundedPaymentsService);
            routing.RouteToEndpoint(typeof(CalculatedRequiredIncentiveAmount), EndpointNames.NonLevyFundedPaymentsService);
        }
    }
}
