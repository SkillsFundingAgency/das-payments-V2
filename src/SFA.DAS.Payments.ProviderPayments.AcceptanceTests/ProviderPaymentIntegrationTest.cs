using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Messages.Commands;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests
{

    [TestFixture]
    public class ProviderPaymentIntegrationTest
    {
        private IEndpointInstance sender;
        private SfaCoInvestedFundingSourcePaymentEvent fundingSourcePaymentEvent;
        private PerformMonthEndProcessingCommand performMonthEndProcessingCommand;
        private IlrSubmittedEvent ilrSubmittedEvent;

        [SetUp]
        public void SetUpAsync()
        {

            fundingSourcePaymentEvent = new SfaCoInvestedFundingSourcePaymentEvent
            {
                ContractType = ContractType.ContractWithSfa,
                FundingSourceType = FundingSourceType.CoInvestedSfa,
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                CollectionPeriod = new CalendarPeriod(2018, 10),
                DeliveryPeriod = new CalendarPeriod(2018, 11),
                LearningAim = new LearningAim
                {
                    FrameworkCode = 1,
                    Reference = "100",
                    PathwayCode = 1,
                    StandardCode = 1,
                    ProgrammeType = 1,
                    AgreedPrice = 1000m,
                    FundingLineType = "T"
                },
                Learner = new Learner
                {
                    Ukprn = 100000,
                    ReferenceNumber = "A1000",
                    Uln = 10000000
                },
                SfaContributionPercentage = 0.9m,
                Ukprn = 100000,
                AmountDue = 5000m,
                PriceEpisodeIdentifier = "2018-P1",
                EventTime = DateTime.UtcNow,
                JobId = 6000,
                IlrSubmissionDateTime = DateTime.UtcNow
            };

            performMonthEndProcessingCommand = new PerformMonthEndProcessingCommand
            {
                JobId = 1,
                CollectionPeriod = new CalendarPeriod(2018, 10)
            };

            ilrSubmittedEvent = new IlrSubmittedEvent
            {
                Ukprn = fundingSourcePaymentEvent.Ukprn,
                EventTime = fundingSourcePaymentEvent.EventTime,
                JobId = fundingSourcePaymentEvent.JobId,
                IlrSubmissionDateTime = DateTime.UtcNow,
                Learner = fundingSourcePaymentEvent.Learner,
                LearningAim = fundingSourcePaymentEvent.LearningAim,
                CollectionPeriod = fundingSourcePaymentEvent.CollectionPeriod
            };

        }

        [Test]
        public async Task ShouldReceiveFundingSourcePaymentEvent()
        {
            sender = await CreateMessageSender<SfaCoInvestedFundingSourcePaymentEvent>();
            await sender.Send(fundingSourcePaymentEvent).ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldReceiveMonthEndEvent()
        {
            sender = await CreateMessageSender<PerformMonthEndProcessingCommand>();
            await sender.Send(performMonthEndProcessingCommand).ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldReceiveIlrSubmissionEvent()
        {
            sender = await CreateMessageSender<IlrSubmittedEvent>();
            await sender.Send(ilrSubmittedEvent).ConfigureAwait(false);
        }

        private async Task<IEndpointInstance> CreateMessageSender<T>()
        {
            var endpointConfiguration = new EndpointConfiguration(EndpointNames.AcceptanceTestEndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));

            endpointConfiguration
                .DisableFeature<TimeoutManager>();

            endpointConfiguration
                .UseTransport<AzureServiceBusTransport>()
                .ConnectionString(TestConfiguration.ServiceBusConnectionString)
                .UseForwardingTopology()
                .Routing()
                .RouteToEndpoint(typeof(T), EndpointNames.ProviderPaymentEndPointName);

            endpointConfiguration.SendFailedMessagesTo("sfa-das-payments-providerpayments-errors");
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseContainer<AutofacBuilder>();

            return await Endpoint.Start(endpointConfiguration);
        }

    }


}
