using NServiceBus;
using NServiceBus.Features;
using NUnit;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Messages.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Tests
{
    [TestFixture]
    public class FundingSourceIntegrationTest
    {
        protected static IEndpointInstance _sender;
        protected ICalculatedPaymentDueEvent _calculatedPaymentDueEvent;

        [OneTimeSetUp]
        public static async Task SetUpMessaging()
        {
            _sender = await CreateMessageSender();
        }

        [SetUp]
        public void SetUp()
        {
            _calculatedPaymentDueEvent = new CalculatedPaymentDueEvent
            {
                JobId = "J000595959",
                EventTime = DateTimeOffset.UtcNow,
                PaymentDueEntity = new PaymentDueEntity()
            };
        }

        [Test]
        public async Task ShouldSendCalculatedPaymentDueEvent()
        {
            await _sender.Send(_calculatedPaymentDueEvent).ConfigureAwait(false);
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
                .RouteToEndpoint(typeof(IPaymentsDueEvent).Assembly, EndpointNames.Nonlevyfundedpaymentsservice);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.UseContainer<AutofacBuilder>();
            endpointConfiguration.SendOnly();

            return await Endpoint.Start(endpointConfiguration);
        }
    }
}