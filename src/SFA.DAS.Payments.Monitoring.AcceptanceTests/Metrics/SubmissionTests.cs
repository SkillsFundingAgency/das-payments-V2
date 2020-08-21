using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Metrics
{
    [TestFixture]
    public class SubmissionTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void CleanUp()
        {

        }

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
            EndpointConfiguration = new EndpointConfiguration(config.AcceptanceTestsEndpointName);
            EndpointConfiguration.SendFailedMessagesTo(config.AcceptanceTestsEndpointName + "-errors");
            EndpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            EndpointConfiguration.EnableInstallers();
            Builder.RegisterInstance(EndpointConfiguration)
                .SingleInstance();
            var conventions = EndpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());
            conventions.DefiningEventsAs(type => type.IsEvent<IPaymentsEvent>());
            conventions.DefiningCommandsAs(type => type.IsCommand<PaymentsCommand>());

            var persistence = EndpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);

            EndpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = EndpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.UseForwardingTopology();
            transport
                .ConnectionString(config.GetConnectionString("ServiceBusConnectionString"))
                .Transactions(TransportTransactionMode.ReceiveOnly);

            transport.Routing().RouteToEndpoint(typeof(GenerateSubmissionSummary).Assembly, "sfa-das-payments-monitoring-metrics-submission");

            EndpointConfiguration.SendOnly();
            Builder.Register(c => new JobsDataContext(config.PaymentsConnectionString))
                .InstancePerDependency();

            Container = Builder.Build();
            EndpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(Container));
            MessageSession = await Endpoint.Start(EndpointConfiguration);
        }


        [TestCase(1)]
        public async Task GenerateMetricsFromLatestJob(int count)
        {
            var dataContext = Container.Resolve<JobsDataContext>();
            var jobs = await dataContext.Jobs
                .Where(job =>
                    job.JobType == JobType.EarningsJob &&
                    (job.Status == JobStatus.Completed || job.Status == JobStatus.CompletedWithErrors)
                ).OrderByDescending(job => job.LearnerCount)
                .Take(count)
                .ToListAsync();

            var messages = jobs.Select(job => new GenerateSubmissionSummary
            {
                Ukprn = job.Ukprn.Value,
                CollectionPeriod = job.CollectionPeriod,
                AcademicYear = job.AcademicYear,
                JobId = job.DcJobId.Value,
            })
                .ToList();

            foreach (var generateSubmissionSummary in messages)
            {
                await MessageSession.Send(generateSubmissionSummary).ConfigureAwait(false);
            }
        }
    }
}