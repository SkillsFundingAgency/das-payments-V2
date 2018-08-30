using NServiceBus;
using NServiceBus.Features;
using NUnit;
using NUnit.Framework;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Tests
{
    [TestFixture]
    public class FundingSourceIntegrationTest
    {
        protected static IEndpointInstance _sender;
        protected RequiredPaymentEvent _requiredPaymentEvent;

        [OneTimeSetUp]
        public static async Task SetUpMessaging()
        {
            _sender = await CreateMessageSender();
        }

        [SetUp]
        public void SetUp()
        {
            _requiredPaymentEvent = new RequiredPaymentEvent
            {
                JobId = "J000595959",
                EventTime = DateTimeOffset.UtcNow,
                Amount = 1000
            };
        }

        [Test]
        public async Task ShouldSendCalculatedPaymentDueEvent()
        {
            await _sender.Send(_requiredPaymentEvent).ConfigureAwait(false);
        }

        private static async Task<IEndpointInstance> CreateMessageSender()
        {
            var endpointConfiguration = new EndpointConfiguration("nonlevyfundedservice-payments-test-sender");
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
            endpointConfiguration.UsePersistence<AzureStoragePersistence>().ConnectionString(TestConfiguration.StorageConnectionString);
            endpointConfiguration.DisableFeature<TimeoutManager>();
            endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();
            endpointConfiguration.UseTransport<AzureStorageQueueTransport>()
                .ConnectionString(TestConfiguration.StorageConnectionString)
                .Routing()
                .RouteToEndpoint(typeof(IRequiredPayment).Assembly, EndpointNames.Nonlevyfundedpaymentsservice);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.UseContainer<AutofacBuilder>();
            endpointConfiguration.SendOnly();

            return await Endpoint.Start(endpointConfiguration);
        }
    }
}