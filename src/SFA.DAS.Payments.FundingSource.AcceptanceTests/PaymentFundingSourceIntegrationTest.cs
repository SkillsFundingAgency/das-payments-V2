using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests
{
    [TestFixture]
    public class FundingSourceIntegrationTest
    {
        private IEndpointInstance sender;
        private RequiredPaymentEvent requiredPaymentEvent;

        [SetUp]
        public async Task SetUpAsync()
        {
            sender = await CreateMessageSender();

            requiredPaymentEvent = new ApprenticeshipContractType2RequiredPaymentEvent
            {
                JobId = "J000595959",
                EventTime = DateTimeOffset.UtcNow,
                AmountDue = 1000.00m
            };
        }

        [Test]
        public async Task ShouldSendCalculatedPaymentDueEvent()
        {
            await sender.Publish(requiredPaymentEvent).ConfigureAwait(false);
        }

        private async Task<IEndpointInstance> CreateMessageSender()
        {
            var endpointConfiguration = new EndpointConfiguration("nonlevyfundedservice-payments-test-sender");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
            conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));
            conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Events") ?? false));

            endpointConfiguration.DisableFeature<TimeoutManager>();
            endpointConfiguration.UseTransport<AzureServiceBusTransport>()
                .UseForwardingTopology()
                                 .ConnectionString(TestConfiguration.ServiceBusConnectionString);
            endpointConfiguration.SendFailedMessagesTo("nonlevyfundedserviceFailedMessagesQueue");
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseContainer<AutofacBuilder>();

            return await Endpoint.Start(endpointConfiguration);
        }
    }
}