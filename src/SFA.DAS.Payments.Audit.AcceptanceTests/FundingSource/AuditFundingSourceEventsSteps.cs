using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Audit.AcceptanceTests.Data;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.FundingSource
{
    [Binding]
    public class AuditFundingSourceEventsSteps : StepsBase
    {

        protected List<FundingSourcePaymentEvent> Events
        {
            get => Get<List<FundingSourcePaymentEvent>>();
            set => Set(value);
        }

        public AuditFundingSourceEventsSteps(ScenarioContext context) : base(context)
        {
        }

        [Given(@"the funding source service has calculated the following payments")]
        public void GivenTheFundingSourceServiceHasCalculatedTheFollowingPayments(Table table)
        {
            var source = table.CreateSet<FundingSourceTestData>();
            Events = source.Select(CreateFundingSourcePaymentEvent).ToList();
        }

        protected FundingSourcePaymentEvent CreateFundingSourcePaymentEvent(FundingSourceTestData testData)
        {
            FundingSourcePaymentEvent fundingSourceEvent;
            switch (testData.FundingSource)
            {
                case FundingSourceType.CoInvestedEmployer:
                    fundingSourceEvent = new EmployerCoInvestedFundingSourcePaymentEvent();
                    break;
                case FundingSourceType.CoInvestedSfa:
                    fundingSourceEvent = new SfaCoInvestedFundingSourcePaymentEvent();
                    break;
                case FundingSourceType.FullyFundedSfa:
                    fundingSourceEvent = new SfaFullyFundedFundingSourcePaymentEvent();
                    break;
                default:
                    throw new InvalidOperationException($"Unhandled funding source type: {testData.FundingSource:G}");
            }

            fundingSourceEvent.TransactionType = testData.TransactionType;
            fundingSourceEvent.ContractType = testData.ContractType;
            fundingSourceEvent.FundingSourceType = testData.FundingSource;
            fundingSourceEvent.RequiredPaymentEventId = Guid.NewGuid();
            fundingSourceEvent.SfaContributionPercentage = .9M;
            fundingSourceEvent.Learner = new Learner
            {
                ReferenceNumber = TestSession.Learner.LearnRefNumber,
                Uln = TestSession.Learner.Uln
            };
            fundingSourceEvent.AmountDue = testData.Amount;
            fundingSourceEvent.CollectionPeriod = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build();
            fundingSourceEvent.DeliveryPeriod = 1;
            fundingSourceEvent.IlrSubmissionDateTime = TestSession.IlrSubmissionTime;
            fundingSourceEvent.JobId = TestSession.JobId;
            fundingSourceEvent.LearningAim = new LearningAim
            {
                FundingLineType = TestSession.Learner.Course.FundingLineType,
                FrameworkCode = TestSession.Learner.Course.FrameworkCode,
                ProgrammeType =  TestSession.Learner.Course.ProgrammeType,
                StandardCode = TestSession.Learner.Course.StandardCode,
                PathwayCode = TestSession.Learner.Course.PathwayCode,
                Reference = TestSession.Learner.Course.LearnAimRef
            };
            fundingSourceEvent.PriceEpisodeIdentifier = "pe-1";
            fundingSourceEvent.Ukprn = TestSession.Ukprn;

            return fundingSourceEvent;
        }

        [When(@"the Audit Funding Source Service is notified of the calculated funding source payments")]
        public async Task WhenTheAuditFundingSourceServiceIsNotifiedOfTheCalculatedFundingSourcePayments()
        {
            foreach (var fundingSourcePaymentEvent in Events)
            {
                Console.WriteLine($"Sending funding source payment event: {fundingSourcePaymentEvent.ToJson()}");
                await MessageSession.Send(fundingSourcePaymentEvent).ConfigureAwait(false);
            }
        }

        [Then(@"the calculated funding source payments should be recorded in the funding source tables")]
        public async Task ThenTheCalculatedFundingSourcePaymentsShouldBeRecordedInTheFundingSourceTables()
        {
            await WaitForIt(async () =>
            {
                var dataContext = Container.Resolve<AuditDataContext>();
                var storedEvents = await dataContext
                    .FundingSourceEvents
                    .Where(fse => fse.JobId == TestSession.JobId)
                    .ToListAsync();
                return Events.All(ev => storedEvents.Any(storedEvent => storedEvent.EventId == ev.EventId));
            },$"Failed to find the funding source events. Job Id: {TestSession.JobId}, Ukprn: {TestSession.Ukprn}");
        }
    }
}
