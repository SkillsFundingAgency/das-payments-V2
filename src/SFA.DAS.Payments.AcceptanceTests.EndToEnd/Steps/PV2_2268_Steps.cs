using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2268-Break-in-Learning-after-redundancy-with-delivered-days-under-75-percent-then-re-employed-before-12-weeks-is-up")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2268_Steps : FM36BreakInLearningBaseSteps
    {
        public PV2_2268_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifierR04 = "PE-2268-R04";

        [Then("the learner must be funded from the employer levy funds")]
        public async Task LearnerFundedFromLevy()
        {
            await WaitForIt(() => HasCorrectlyFundedPaymentFromLevyForR04(),
                "Failed to find learning payment funded from Levy for R04");
        }

        private bool HasCorrectlyFundedPaymentFromLevyForR04()
        {
            return FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(PriceEpisodeIdentifierR04, short.Parse(TestSession.FM36Global.Year), TestSession)
                .Any(x =>
                    x.FundingSourceType == FundingSourceType.Levy
                    && x.DeliveryPeriod == 4
                    && x.TransactionType == TransactionType.Learning);
        }
    }
}