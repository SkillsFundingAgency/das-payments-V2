using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ACT2On_ProgrammePaymentsSteps : StepsBase
    {
        protected ApprenticeshipContractType2EarningEvent EarningEvent
        {
            get => Get<ApprenticeshipContractType2EarningEvent>();
            set => Set(value);
        }

        [Given(@"the learner has some on-programme earnings")]
        public void GivenTheLearnerHasSomeOn_ProgrammeEarnings()
        {
            EarningEvent = new ApprenticeshipContractType2EarningEvent
            {
                JobId = "job-1234",
                Ukprn = 1,
                EventTime = DateTimeOffset.UtcNow,
                Learner = new Learner {ReferenceNumber = "12345", Uln = 12345},
                LearningAim =
                    new LearningAim
                    {
                        AgreedPrice = 5000,
                        FrameworkCode = 1234,
                        PathwayCode = 1234,
                        ProgrammeType = 1,
                        Reference = "Ref-1234",
                        StandardCode = 1
                    },
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        StartDate = DateTime.Now.AddMonths(-6),
                        EndDate = DateTime.Now.AddMonths(-3),
                        AgreedPrice = 15000,
                        Identifier = "p-1",
                    },
                    new PriceEpisode
                    {
                        StartDate = DateTime.Now.AddMonths(-3),
                        EndDate = DateTime.Now.AddMonths(6),
                        AgreedPrice = 15000,
                        Identifier = "p-1",
                    }
                }.AsReadOnly(),
                EarningYear = (short) DateTime.Today.Year,
                SfaContributionPercentage = 0.9M,
                IncentiveEarnings =
                    new List<IncentiveEarning>
                    {
                        new IncentiveEarning
                        {
                            Type = IncentiveType.LearningSupport,
                            Periods = new List<EarningPeriod> {new EarningPeriod {Amount = 500, Period = 1}}
                                .AsReadOnly()
                        }
                    }.AsReadOnly(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new List<EarningPeriod>
                        {
                            new EarningPeriod {Amount = 1000, Period = 1},
                            new EarningPeriod {Amount = 1000, Period = 2},
                            new EarningPeriod {Amount = 1000, Period = 3},
                            new EarningPeriod {Amount = 1000, Period = 4},
                            new EarningPeriod {Amount = 1000, Period = 5},
                            new EarningPeriod {Amount = 1000, Period = 6},
                            new EarningPeriod {Amount = 1000, Period = 7},
                            new EarningPeriod {Amount = 1000, Period = 8},
                            new EarningPeriod {Amount = 1000, Period = 9},
                            new EarningPeriod {Amount = 1000, Period = 10},
                            new EarningPeriod {Amount = 1000, Period = 11},
                            new EarningPeriod {Amount = 1000, Period = 12}
                        }.AsReadOnly()
                    }
                }.AsReadOnly()
            };
        }
        
        [When(@"the earnings are sent to the required payments service")]
        public async Task WhenTheEarningsAreSentToTheRequiredPaymentsService()
        {
            var paymentDueEvents = EarningToPaymentDue(EarningEvent);
            foreach (var paymentDueEvent in paymentDueEvents)
            {
                await MessageSession.Send(paymentDueEvent).ConfigureAwait(false);
            }
            
        }
        
        [Then(@"the service should generate the required payments")]
        public void ThenTheServiceShouldGenerateTheRequiredPayments()
        {
            WaitForIt(() =>
            {
                var periods = EarningEvent.OnProgrammeEarnings.SelectMany(earning => earning.Periods,(earning,period)=> new {earning,period});
                return periods.All(earningPeriod => ApprenticeshipContractType2Handler.ReceivedEvents.Any(receivedEvent => receivedEvent.AmountDue == earningPeriod.period.Amount && receivedEvent.DeliveryPeriod?.Period == earningPeriod.period.Period));
            },"Failed to find all the required payment earning events");
        }

        public static List<ApprenticeshipContractType2PaymentDueEvent> EarningToPaymentDue(ApprenticeshipContractType2EarningEvent earning)
        {
            var events = new List<ApprenticeshipContractType2PaymentDueEvent>();

            foreach (var onProgrammeEarning in earning.OnProgrammeEarnings)
            {
                foreach (var period in onProgrammeEarning.Periods)
                {
                    events.Add(new ApprenticeshipContractType2PaymentDueEvent
                    {
                        Learner = earning.Learner,
                        Ukprn = earning.Ukprn,
                        TransactionType = (int)earning.OnProgrammeEarnings.First().Type,
                        DeliveryPeriod = new CalendarPeriod("1819", period.Period),
                        LearningAim = earning.LearningAim,
                        PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                        CollectionPeriod = new CalendarPeriod("1819-R01"),
                        AmountDue = period.Amount,
                        JobId = earning.JobId
                    });
                }
            }

            return events;
        }
    }
}
