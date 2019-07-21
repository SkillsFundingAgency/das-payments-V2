using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Bogus;
using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.AcceptanceTests.Core.Services;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;

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
            routing.RouteToEndpoint(typeof(RecordStartedProcessingEarningsJob), EndpointNames.JobMonitoring);

            var sanitization = transportConfig.Sanitization();
            var strategy = sanitization.UseStrategy<ValidateAndHashIfNeeded>();
            strategy.RuleNameSanitization(
                ruleNameSanitizer: ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            EndpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            EndpointConfiguration.EnableInstallers();
            


            Builder.RegisterType<EarningsJobClient>()
                .As<IEarningsJobClient>()
                .SingleInstance();

            Builder.RegisterType<EarningsJobClientFactory>()
                .As<IEarningsJobClientFactory>()
                .SingleInstance();

            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new TestPaymentsDataContext(configHelper.PaymentsConnectionString);
            }).As<IPaymentsDataContext>().AsSelf().InstancePerDependency();
            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new JobsDataContext(configHelper.PaymentsConnectionString);
            }).InstancePerDependency();
            DcHelper.AddDcConfig(Builder);
            Builder.RegisterType<EndpointInstanceFactory>()
                .As<IEndpointInstanceFactory>()
                .SingleInstance();

            Container = Builder.Build();
            EndpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(Container));
            MessageSession = await Container.Resolve<IEndpointInstanceFactory>().GetEndpointInstance().ConfigureAwait(false);//    await Endpoint.Start(EndpointConfiguration);
            LearnerStartTimes = new Dictionary<string, DateTime>();
        }


        [TestCase(1, 100, 1)]
        public async Task Repeatable_Ukprn_And_Uln(int providerCount, int providerLearnerCount, int collectionPeriod)
        {
            Randomizer.Seed = new Random(8675309);
            var sessions = Enumerable.Range(1, providerCount)
                .Select(i => new TestSession(new RandomUkprnService(Container.Resolve<TestPaymentsDataContext>()), new RandomUlnService()))
                .ToList();
            var ilrSubmissions = new List<Task>();
            if (providerLearnerCount > 1)
            {
                DeliveryTime = DateTimeOffset.UtcNow.AddSeconds(providerLearnerCount >= 10000 ? 600 : providerLearnerCount >= 1000 ? 300 : 60);
                Console.WriteLine($"Using delivery time of: {DeliveryTime:O}");
            }

            var learnerId = 0;
            foreach (var session in sessions)
            {
                session.Learners.Clear();
                session.Learners.AddRange(Enumerable.Range(1, providerLearnerCount)
                    .Select(i => new Learner
                    {
                        Ukprn = session.Ukprn,
                        Uln = ++learnerId,
                        LearnRefNumber = learnerId.ToString(),
                        Course = session.CourseFaker.Generate(1).FirstOrDefault()
                    }));
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
            var startTime = DateTimeOffset.UtcNow;
            var sendOptions = new SendOptions();
            if (DeliveryTime.HasValue)
            {
                sendOptions.DoNotDeliverBefore(DeliveryTime.Value);
                startTime = DeliveryTime.Value;
            }

            var commands = ilrLearners.Select(learner => new ProcessLearnerCommand
            {
                JobId = session.JobId,
                CollectionPeriod = collectionPeriod,
                CollectionYear = 1819,
                IlrSubmissionDateTime = session.IlrSubmissionTime,
                SubmissionDate = session.IlrSubmissionTime,
                Ukprn = session.Ukprn,
                Learner = learner
            })
                .ToList();
            var generatedMessages = commands.Select(command => new GeneratedMessage
            {
                StartTime = command.RequestTime,
                MessageName = command.GetType().FullName,
                MessageId = command.CommandId
            }).ToList(); 
            //await CreateJob(session, startTime, generatedMessages, (byte)collectionPeriod, JobType.EarningsJob).ConfigureAwait(false);
            //var jobClient = Container.Resolve<IEarningsJobClient>();
            //await jobClient.StartJob(session.JobId, session.Ukprn, session.IlrSubmissionTime, 1819,
            //    (byte) collectionPeriod, generatedMessages, startTime);
            //var jobMessage = new RecordStartedProcessingEarningsJob
            //{
            //    Ukprn = session.Ukprn,
            //    CollectionPeriod = (byte)collectionPeriod,
            //    GeneratedMessages = generatedMessages,
            //    IlrSubmissionTime = session.IlrSubmissionTime,
            //    JobId = session.JobId,
            //    StartTime = startTime,
            //    LearnerCount = generatedMessages.Count

            //};


            //foreach (var processLearnerCommand in commands)
            //{
            //    await MessageSession.Send(processLearnerCommand, sendOptions).ConfigureAwait(false);
            //    Console.WriteLine($"sent process learner command. Uln: {processLearnerCommand.Learner.ULN}");
            //}

            var dcHelper = Container.Resolve<DcHelper>();
            await dcHelper.SendIlrSubmission(ilrLearners, session.Ukprn, 1819, (byte) collectionPeriod, session.JobId);
        }

        [OneTimeTearDown]
        public void CleanUpContainer()
        {

        }

        public async Task CreateJob(TestSession session,  DateTimeOffset startTime, List<GeneratedMessage> generatedMessages, byte collectionPeriod, JobType jobType = JobType.ComponentAcceptanceTestEarningsJob)
        {
            var job = new JobModel
            {
                CollectionPeriod = collectionPeriod,
                AcademicYear = 1819,
                StartTime = startTime,
                Ukprn = session.Ukprn,
                DcJobId = session.JobId,
                IlrSubmissionTime = session.IlrSubmissionTime,
                JobType = JobType.ComponentAcceptanceTestEarningsJob,
                LearnerCount = generatedMessages.Count,
                Status = JobStatus.InProgress
            };
            var dataContext = Container.Resolve<JobsDataContext>();
            dataContext.Jobs.Add(job);
            await dataContext.SaveChangesAsync();
            Console.WriteLine($"Saved new test job to database. Job Id: {job.Id}");
            dataContext.JobSteps.AddRange(generatedMessages.Select(msg => new JobStepModel
            {
                JobId = job.Id,
                StartTime = msg.StartTime,
                MessageName = msg.MessageName,
                MessageId = msg.MessageId,
                Status = JobStepStatus.Queued
            }));
            await dataContext.SaveChangesAsync();
            Console.WriteLine($"Finished creating job and generated messages. Job id: {job.Id}, Test DC Job id: {job.DcJobId}");
        }

    }
}
