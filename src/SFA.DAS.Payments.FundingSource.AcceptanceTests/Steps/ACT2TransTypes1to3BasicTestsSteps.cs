using SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class ACT2TransTypes1to3BasicTestsSteps : FundingSourceStepBase
    {
        protected ACT2TransTypes1to3BasicTestsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Then(@"the payment source component will not generate any contract type (.*) Learning \(TT(.*)\) coinvested payments")]
        public void ThenThePaymentSourceComponentWillNotGenerateAnyContractTypeLearningTTCoinvestedPayments(byte expectedContractType, byte expectedOnProgrammeType)
        {
            WaitForIt(() =>
            {
                return !CoInvestedFundingSourceHandler.ReceivedEvents.Any(receivedEvent =>
                        expectedContractType == receivedEvent.ContractType
                        && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                        && TestSession.Ukprn == receivedEvent.Ukprn
                        && (OnProgrammeEarningType)expectedOnProgrammeType == receivedEvent.OnProgrammeEarningType
                    );
            }, "Failed to find all the funding source payment events");
        }

        [Then(@"the payment source component will not generate any contract type (.*) Balancing \(TT(.*)\) coinvested payments")]
        public void ThenThePaymentSourceComponentWillNotGenerateAnyContractTypeBalancingTTCoinvestedPayments(byte expectedContractType, byte expectedOnProgrammeType)
        {
            WaitForIt(() =>
            {
                return !CoInvestedFundingSourceHandler.ReceivedEvents.Any(receivedEvent =>
                    expectedContractType == receivedEvent.ContractType
                    && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && (OnProgrammeEarningType)expectedOnProgrammeType == receivedEvent.OnProgrammeEarningType
                );
            }, "Failed to find all the funding source payment events");
        }

        [Then(@"the payment source component will not generate any contract type (.*) Completion \(TT(.*)\) coinvested payments")]
        public void ThenThePaymentSourceComponentWillNotGenerateAnyContractTypeCompletionTTCoinvestedPayments(byte expectedContractType, byte expectedOnProgrammeType)
        {
            WaitForIt(() =>
            {
                return !CoInvestedFundingSourceHandler.ReceivedEvents.Any(receivedEvent =>
                    expectedContractType == receivedEvent.ContractType
                    && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && (OnProgrammeEarningType)expectedOnProgrammeType == receivedEvent.OnProgrammeEarningType
                );
            }, "Failed to find all the funding source payment events");
        }

    }
}
