using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class DuplicationSteps : StepsBase
    {
        private const string IdentifiedRemovedLearningAim = "IdentifiedRemovedLearningAim";
        private const string JobIds = "JobIds";
        private const string HistoricPayment = "HistoricPaymentModel";

        public DuplicationSteps(ScenarioContext context) : base(context)
        {
        }

        [Given("a learner has a payment from a previous submission")]
        public async Task InsertHistoricPayment()
        {
            var historicPayment = new PaymentModel
            {
                EventId = Guid.NewGuid(),
                LearningAimFrameworkCode = 1,
                LearningAimFundingLineType = "funding line type",
                LearningAimPathwayCode = 2,
                LearningAimProgrammeType = 3,
                LearningAimReference = "ZPROG001",
                LearningAimStandardCode = 4,
                StartDate = DateTime.Now.AddMonths(-2),
                Amount = 100m,
                CollectionPeriod = new CollectionPeriod{AcademicYear = 1920, Period = 1},
                ContractType = ContractType.Act2,
                PriceEpisodeIdentifier = "pe-1",
                DeliveryPeriod = 1,
                LearnerReferenceNumber = "abc",
                LearnerUln = 123456,
                Ukprn = TestSession.GenerateId(),
                FundingSource = FundingSourceType.CoInvestedSfa,
                JobId = TestSession.GenerateId(),
                TransactionType = TransactionType.Learning,
            };

            Context.Add(HistoricPayment, historicPayment);
            var dataContext = Container.Resolve<IPaymentsDataContext>();
            await dataContext.Payment.AddAsync(historicPayment);
            await dataContext.SaveChangesAsync();
        }

        [When("an idenitfied removed learning aim event is handled by the required payments service")]
        public async Task HandleRemovedLearner()
        {
            var payment = Context.Get<PaymentModel>(HistoricPayment);

            var removedLearnerEvent = new IdentifiedRemovedLearningAim
            { 
                JobId = TestSession.GenerateId(),
                CollectionPeriod = new CollectionPeriod{AcademicYear = 1920, Period = 5},
                IlrSubmissionDateTime = DateTime.Now,
                Ukprn = payment.Ukprn,
                ContractType = ContractType.Act2,
                LearningAim = new LearningAim
                {
                    FrameworkCode = 1,
                    FundingLineType = "funding line type",
                    PathwayCode = 2,
                    ProgrammeType = 3,
                    Reference = "ZPROG001",
                    StandardCode = 4,
                    StartDate = DateTime.Now.AddMonths(-2),
                },
                Learner = new Learner
                {
                    ReferenceNumber = "abc",
                }
            };

            Context.Add(IdentifiedRemovedLearningAim, removedLearnerEvent);
            Context.Add(JobIds, new List<long>{ removedLearnerEvent.JobId});

            await MessageSession.Publish(removedLearnerEvent);
        }

        [When("the event is duplicated")]
        public async Task DuplicateEvent()
        {
            var @event = Context.Get<IdentifiedRemovedLearningAim>(IdentifiedRemovedLearningAim);
            await MessageSession.Publish(@event);
        }

        [When("the event is resubmitted with a different EventId")]
        public async Task ResubmitTheEvent()
        {
            var @event = Context.Get<IdentifiedRemovedLearningAim>(IdentifiedRemovedLearningAim);
            @event.EventId = Guid.NewGuid();
            await MessageSession.Publish(@event);
        }

        [Then("there is only a single event produced")]
        public void IgnoreTheDuplicate()
        {
            var @event = Context.Get<IdentifiedRemovedLearningAim>(IdentifiedRemovedLearningAim);
            var jobid = @event.JobId;
            PeriodisedRequiredPaymentEventHandler
                .ReceivedEvents.Where(x => x.JobId == jobid)
                .Should().HaveCount(1);
        }

        [Then("after waiting a second")]
        public Task WaitASecond()
        {
            return Task.Delay(TimeSpan.FromSeconds(1));
        }

        [Then("only one set of events is generated for the learner")]
        public async Task CheckForDuplicateEvents()
        {
            var @event = Context.Get<IdentifiedRemovedLearningAim>(IdentifiedRemovedLearningAim);
            var jobid = @event.JobId;
            await WaitForIt(() => PeriodisedRequiredPaymentEventHandler
                    .ReceivedEvents.Count(x => x.JobId == jobid) == 1,
                "Failed to find exactly one event");
        }
    }
}
