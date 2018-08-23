using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events.OnProgramme;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Tests
{
    //this is a temporary test just to get messaging working
    [TestFixture]
    public class RequiredPaymentsIntegrationTest
    {
        protected static IEndpointInstance Sender;
        protected IEarningEvent Earning;

        [OneTimeSetUp]
        public static async Task SetUpMessaging()
        {
            Sender = await CreateMessageSender();
        }

        [SetUp]
        public void SetUp()
        {
            Earning = new ApprenticeshipContractType2EarningEvent
            {
                JobId = "job-1234",
                Ukprn = 1,
                EventTime = DateTimeOffset.UtcNow,
                Learner = new Learner
                {
                    ReferenceNumber = "12345",
                    Uln = 12345
                },
                LearningAim = new LearningAim
                {
                    AgreedPrice = 5000,
                    FrameworkCode = 1234,
                    PathwayCode = 1234,
                    ProgrammeType = 1,
                    Reference = "Ref-1234",
                    StandardCode = 1
                },
                PriceEpisodes = new List<OnProgrammeEarningPriceEpisode>
                {
                    new OnProgrammeEarningPriceEpisode
                    {
                        Type = EarningType.Learning,
                        StartDate = DateTime.Now.AddMonths(-6),
                        EndDate = DateTime.Now.AddMonths(6),
                        AgreedPrice = 15000,
                        Identifier = "p-1",
                        Periods = new List<PriceEpisodePeriod>
                        {
                            new PriceEpisodePeriod
                            {
                                Amount = 1000,
                                Periods = new List<byte> {1,2,3,4,5,6,7,8,9,10,11,12}
                            }
                        }
                    },
                    new OnProgrammeEarningPriceEpisode
                    {
                        Type = EarningType.Completion,
                        StartDate = DateTime.Now.AddMonths(-6),
                        EndDate = DateTime.Now.AddMonths(6),
                        AgreedPrice = 15000,
                        Identifier = "p-1",
                        Periods = new List<PriceEpisodePeriod>
                        {
                            new PriceEpisodePeriod
                            {
                                Amount = 3000,
                                Periods = new List<byte> {13}
                            }
                        }
                    }
                }
            };
        }

        [Test]
        public async Task Processes_Earning_Event()
        {
            Console.WriteLine("Sending test earning event");
            await Sender.Send(Earning).ConfigureAwait(false);
            Console.WriteLine("Finished sending earning");
        }

        private static async Task<IEndpointInstance> CreateMessageSender()
        {
            var endpointConfiguration = new EndpointConfiguration("required-payments-test-sender");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
            //conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments")??false) && (type.Namespace?.Contains(".Messages.Commands")??false));
            //conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments")??false) && (type.Namespace?.Contains(".Messages.Events")??false));

            endpointConfiguration.UsePersistence<AzureStoragePersistence>()
                .ConnectionString(TestConfiguration.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();

            endpointConfiguration.UseTransport<AzureStorageQueueTransport>()
                .ConnectionString(TestConfiguration.StorageConnectionString)
                .Routing()
                .RouteToEndpoint(typeof(IEarningEvent).Assembly, EndpointNames.RequiredPayments);

            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            //            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseContainer<AutofacBuilder>();
            endpointConfiguration.SendOnly();
            return await Endpoint.Start(endpointConfiguration);
        }
    }
}