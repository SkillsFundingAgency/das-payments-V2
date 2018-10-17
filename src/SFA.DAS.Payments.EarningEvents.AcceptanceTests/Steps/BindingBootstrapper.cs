using System;
using Autofac;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class BindingBootstrapper : StepsBase
    {
        public BindingBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeTestRun(Order = 51)]
        public static void AddRoutingConfig()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<ApprenticeshipContractType2EarningEvent>());
            var transportConfig = Container.Resolve<TransportExtensions<AzureServiceBusTransport>>();
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(ProcessLearnerCommand), EndpointNames.EarningEventsService);
        }

        //[BeforeTestRun(Order = 25)]
        //public static void AddTopicSubscription()
        //{
        //    Builder.Register(c =>
        //    {
        //        var topicSubscriptionConfig = new TopicConfiguration(TestConfiguration.ServiceBusConnectionString, TestConfiguration.TopicName, TestConfiguration.SubscriptionName, 1, maximumCallbackTimeSpan: TimeSpan.FromMinutes(40));

        //        return new TopicSubscriptionSevice<JobContextDto>(
        //            topicSubscriptionConfig,
        //            c.Resolve<IJsonSerializationService>(),
        //            c.Resolve<IPaymentLogger>());
        //    }).As<ITopicSubscriptionService<JobContextDto>>();
        //}
    }
}
