using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public class DataLockSteps : EndToEndSteps
    {
        public DataLockSteps(FeatureContext context) : base(context)
        {
        }

        [Then(@"the following data lock failures will be generated")]
        public async Task ThenOnlyTheFollowingNonPayableEarningsWillBeGenerated(Table table)
        {
            var dataLockErrors = table.CreateSet<DataLockError>().ToList();
            var matcher = new DataLockErrorMatcher(TestSession.Provider, TestSession, dataLockErrors);
            await WaitForIt(() => matcher.MatchPayments(), "Provider Payment event check failure");
        }
    }

    public class DataLockErrorMatcher : BaseMatcher<NonPayableEarningEvent>
    {
        private IList<DataLockError> expectedErrors;
        private TestSession testSession;
        private Provider provider;

        public DataLockErrorMatcher(Provider provider, TestSession testSession, IList<DataLockError> expectedErrors)
        {
            this.expectedErrors = expectedErrors;
            this.provider = provider;
            this.testSession = testSession;
        }

        protected override IList<NonPayableEarningEvent> GetActualEvents()
        {
            throw new NotImplementedException();
        }

        protected override IList<NonPayableEarningEvent> GetExpectedEvents()
        {
            var learner = testSession.GetLearner(provider.Ukprn, expectedErrors[0].LearnerId);

            var nonPayableEarning = new NonPayableEarningEvent
            {
                Ukprn = provider.Ukprn,
                Learner = new Model.Core.Learner
                {
                    Uln = learner.Uln
                },
                LearningAim = new LearningAim
                {
                    // framework code
                    // programme type
                    // pathway code
                },
                OnProgrammeEarnings = expectedErrors.GroupBy(e => e.TransactionType).Select(errorSpec =>
                {
                    return new OnProgrammeEarning
                    {
                        Type = (OnProgrammeEarningType) errorSpec.Key,
                        Periods = errorSpec.Select(period => new EarningPeriod
                        {
                            Period = new CollectionPeriodBuilder().WithSpecDate(period.DeliveryPeriod).Build().Period
                        }).ToList().AsReadOnly()
                    };
                }).ToList()
            };
        }

        protected override bool Match(NonPayableEarningEvent expected, NonPayableEarningEvent actual)
        {
            throw new NotImplementedException();
        }
    }
}
