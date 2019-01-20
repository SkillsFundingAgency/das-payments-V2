using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ACT2On_ProgrammePaymentsSteps : StepsBase
    {
        public ACT2On_ProgrammePaymentsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        protected ApprenticeshipContractType2EarningEvent EarningEvent
        {
            get => Get<ApprenticeshipContractType2EarningEvent>();
            set => Set(value);
        }


        [Given(@"the learner has some on-programme earnings")]
        public void GivenTheLearnerHasSomeOn_ProgrammeEarnings()
        {
            SetCurrentCollectionYear();

            EarningEvent = new ApprenticeshipContractType2EarningEvent
            {
                JobId = TestSession.JobId,
                Ukprn = TestSession.Ukprn,
                Learner = new Learner {ReferenceNumber = "12345", Uln = 12345},
                LearningAim = new LearningAim
                {
                    FrameworkCode = 1234,
                    PathwayCode = 1234,
                    ProgrammeType = 1,
                    Reference = "Ref-1234",
                    StandardCode = 1,
                    FundingLineType = "Funding-LineType"
                },
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1718, 10),
                CollectionYear = AcademicYear,
                SfaContributionPercentage = 0.90m,
                OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = 10,
                                Amount = 1000,
                                PriceEpisodeIdentifier = "p1-1"
                            }
                        })
                    }
                })
            };
        }

        [When(@"the earnings are sent to the required payments service")]
        public async Task WhenTheEarningsAreSentToTheRequiredPaymentsService()
        {            
            await MessageSession.Send(EarningEvent).ConfigureAwait(false);
        }
        
        [Then(@"the service should generate the required payments")]
        public async Task ThenTheServiceShouldGenerateTheRequiredPayments()
        {
            await WaitForIt(() =>
            {
                return ApprenticeshipContractType2Handler.ReceivedEvents
                    .Any(receivedEvent =>
                    {
                        var spec = EarningEvent.OnProgrammeEarnings[0].Periods.SingleOrDefault(p => p.Period == receivedEvent.DeliveryPeriod);

                        if (spec == null)
                            return false;

                        return receivedEvent.AmountDue == spec.Amount &&
                               receivedEvent.Ukprn == TestSession.Ukprn &&
                               receivedEvent.JobId == TestSession.JobId;
                    });
            },"Failed to find all the required payment events");
        }
    }
}
