using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Bogus;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.PerformanceTests
{
    [TestFixture]
    public class EndToEndPerformanceTests : TestsBase
    {
        protected IMessageSession MessageSession { get; private set; }
        protected EndpointConfiguration EndpointConfiguration { get; private set; }
        protected IContainer Container { get; private set; }
        protected Autofac.ContainerBuilder Builder { get; private set; }
        protected Dictionary<string, DateTime> LearnerStartTimes { get; private set; }

        [NUnit.Framework.OneTimeSetUp]
        public async Task SetUpContainer()
        {
            var config = new TestsConfiguration();
            Builder = new ContainerBuilder();
            Builder.RegisterType<TestsConfiguration>().SingleInstance();
            Builder.RegisterType<DcHelper>().SingleInstance();
            EndpointConfiguration = new EndpointConfiguration(config.AcceptanceTestsEndpointName);
            Builder.RegisterInstance(EndpointConfiguration)
                .SingleInstance();
            var conventions = EndpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());
            conventions.DefiningEventsAs(type => type.IsEvent<IPaymentsEvent>());
            conventions.DefiningCommandsAs(type => type.IsCommand<PaymentsCommand>());

            EndpointConfiguration.UsePersistence<AzureStoragePersistence>()
                .ConnectionString(config.StorageConnectionString);
            EndpointConfiguration.DisableFeature<TimeoutManager>();

            var transportConfig = EndpointConfiguration.UseTransport<AzureServiceBusTransport>();
            Builder.RegisterInstance(transportConfig)
                .As<TransportExtensions<AzureServiceBusTransport>>()
                .SingleInstance();
            transportConfig
                .UseForwardingTopology()
                .ConnectionString(config.ServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly);
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(ProcessLearnerCommand), EndpointNames.EarningEvents);
            routing.RouteToEndpoint(typeof(ProcessProviderMonthEndCommand), EndpointNames.ProviderPayments);

            var sanitization = transportConfig.Sanitization();
            var strategy = sanitization.UseStrategy<ValidateAndHashIfNeeded>();
            strategy.RuleNameSanitization(
                ruleNameSanitizer: ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            EndpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            EndpointConfiguration.EnableInstallers();

            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new PaymentsDataContext(configHelper.PaymentsConnectionString);
            }).As<IPaymentsDataContext>().InstancePerDependency();
            DcHelper.AddDcConfig(Builder);

            Container = Builder.Build();
            EndpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(Container));
            MessageSession = await Endpoint.Start(EndpointConfiguration);
            LearnerStartTimes = new Dictionary<string, DateTime>();
        }


        [TestCase(5, 500, 1)]
        public async Task Repeatable_Ukprn_And_Uln(int providerCount, int providerLearnerCount, int collectionPeriod)
        {
            Randomizer.Seed = new Random(8675309);
            var sessions = Enumerable.Range(1, providerCount)
                .Select(i => new TestSession(i))
                .ToList();
            var ilrSubmissions = new List<Task>();
            if (providerLearnerCount > 1)
            {
                DeliveryTime = DateTimeOffset.UtcNow.AddSeconds(providerLearnerCount >= 10000 ? 600 : providerLearnerCount >= 1000 ? 300 : 60);
                Console.WriteLine($"Using delivery time of: {DeliveryTime:O}");
            }

            var learnerId = 1;
            foreach (var session in sessions)
            {
                session.Learners.Clear();
                session.Learners.AddRange(Enumerable.Range(1, providerLearnerCount)
                    .Select(i => session.GenerateLearner(learnerId++)));
                ilrSubmissions.Add(SubmitIlr(session, collectionPeriod));
                await Task.WhenAll(ilrSubmissions);
                Console.WriteLine($"Finished sending Ukprn: {session.Ukprn}. Time: {DateTime.Now:O}");
            }
        }

        protected DateTimeOffset? DeliveryTime;

        protected async Task SubmitIlr(TestSession session, int collectionPeriod)
        {
            var ilrLearners = session.Learners
                .Select(learner => CreateFM36Learner(session, learner))
                .ToList();
            session.IlrSubmissionTime = DateTime.UtcNow;
            foreach (var fm36Learner in ilrLearners)
            {
                var command = new ProcessLearnerCommand
                {
                    Learner = fm36Learner,
                    CollectionPeriod = collectionPeriod,
                    CollectionYear = "1819",
                    Ukprn = session.Ukprn,
                    JobId = session.JobId,
                    IlrSubmissionDateTime = session.IlrSubmissionTime,
                    RequestTime = DateTimeOffset.UtcNow,
                    SubmissionDate = session.IlrSubmissionTime,
                };
                var sendOptions = new SendOptions();
                if (DeliveryTime.HasValue)
                    sendOptions.DoNotDeliverBefore(DeliveryTime.Value);
                await MessageSession.Send(command, sendOptions);
                LearnerStartTimes.Add(fm36Learner.LearnRefNumber, DateTime.Now);
                Console.WriteLine($"Sent learner.  Ukprn: {session.Ukprn}, Learner: {fm36Learner.LearnRefNumber}, Time: {DateTime.Now:o}");
            }
            var dcHelper = Container.Resolve<DcHelper>();
            await dcHelper.SendIlrSubmission(ilrLearners, session.Ukprn, "1819", (byte)collectionPeriod, session.JobId);
        }

        [OneTimeTearDown]
        public void CleanUpContainer()
        {

        }
    }
}
