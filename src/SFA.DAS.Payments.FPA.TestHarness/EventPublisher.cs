﻿using NUnit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.Payments.FPA.Messages.InboundEvents;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using IContainer = NServiceBus.ObjectBuilder.Common.IContainer;

namespace SFA.DAS.Payments.FPA.TestHarness
{
    [TestFixture]
    public class EventPublisher
    {
        public static ContainerBuilder Builder { get; protected set; } // -1
        public static IContainer Container { get; protected set; } // 50
        public static IMessageSession MessageSession { get; protected set; }
        //public static TestsConfiguration Config => Container.Resolve<TestsConfiguration>();
        //public static string Environment => Config.GetAppSetting("Environment");

        private static readonly string StorageConnectionString = SettingsHelper.GetConnectionString("StorageConnectionString");
        private static readonly string ServiceBusConnectionString = SettingsHelper.GetConnectionString("ServiceBusConnectionString");
        private static readonly string FPAEndpointName = SettingsHelper.GetAppSetting("EndpointName");

        private EndpointConfiguration endpointConfiguration;

        [SetUp]
        public void SetUpEndpoint()
        {
            Builder = new ContainerBuilder();
            //var config = new TestsConfiguration();
            endpointConfiguration = new EndpointConfiguration(FPAEndpointName);

            Builder.RegisterInstance(endpointConfiguration)
                .Named<EndpointConfiguration>("FPATestEndpointConfiguration")
                .SingleInstance();
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => type == typeof(LearningPaymentEvent));
            //conventions.DefiningCommandsAs(t => t.IsInNamespace("SFA.DAS.CommitmentsV2.Messages.Events"));

            endpointConfiguration.UsePersistence<AzureStoragePersistence>()
                .ConnectionString(StorageConnectionString);
            endpointConfiguration.DisableFeature<TimeoutManager>();

            var transportConfig = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            Builder.RegisterInstance(transportConfig)
                .Named<TransportExtensions<AzureServiceBusTransport>>("FPATestTransportConfig")
                .SingleInstance();

            transportConfig
                .UseForwardingTopology()
                .ConnectionString(ServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .Queues()
                .DefaultMessageTimeToLive(TimeSpan.FromMinutes(20));
            var routing = transportConfig.Routing();
            routing.RouteToEndpoint(typeof(LearningPaymentEvent).Assembly, FPAEndpointName);

            var sanitization = transportConfig.Sanitization();
            var strategy = sanitization.UseStrategy<ValidateAndHashIfNeeded>();
            strategy.RuleNameSanitization(
                ruleNameSanitizer: ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
        }

        [Test]
        public async Task FireBasicEvent()
        {
            var learningPaymentEvent = new LearningPaymentEvent()
            {
                AccountId = 112,
                ActualEndDate = null,
                AgreedOnDate = DateTime.Now,
                AgreementId = "114",
                AmountDue = 2000,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ApprenticeshipId = 116,
                ApprenticeshipPriceEpisodeId = 118,
                ClawbackSourcePaymentEventId = null,
                CollectionPeriod = new CollectionPeriod{ AcademicYear = 2122, Period = 1 },
                CompletionAmount = 1000,
                CompletionStatus = 0,
                ContractType = ContractType.Act1,
                DeliveryPeriod = 1,
                EarningEventId = Guid.Empty,
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.Now,
                IlrFileName = "Approvals",
                IlrSubmissionDateTime = DateTime.Now,
                InstalmentAmount = 2000,
                JobId = 120,
                Learner = new Learner{ ReferenceNumber = "122", Uln = 124 },
                LearningAim = new LearningAim(), //todo?
                LearningAimSequenceNumber = 126,
                LearningStartDate = DateTime.Now.AddMonths(-1),
                NumberOfInstalments = 24,
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                PlannedEndDate = DateTime.Now.AddYears(2),
                PriceEpisodeIdentifier = "128",
                Priority = 0,
                ReportingAimFundingLineType = "FundingLineType1",
                SfaContributionPercentage = 0.95m,
                StartDate = DateTime.Now.AddMonths(-1),
                TransferSenderAccountId = null,
                Ukprn = 130
            };

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            await endpointInstance.Publish(learningPaymentEvent)
                .ConfigureAwait(false);
        }
    }
}
