using SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;
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
        public async Task ThenThePaymentSourceComponentWillNotGenerateAnyContractTypeLearningTTCoinvestedPayments(ContractType expectedContractType, byte expectedOnProgrammeType)
        {
            await WaitForIt(() =>
            {
                return !FundingSourcePaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                        expectedContractType == receivedEvent.ContractType
                        && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                        && TestSession.Ukprn == receivedEvent.Ukprn
                        && (TransactionType)expectedOnProgrammeType == receivedEvent.TransactionType
                    );
            }, "Failed to find all the funding source payment events");
        }

        [Then(@"the payment source component will not generate any contract type (.*) Balancing \(TT(.*)\) coinvested payments")]
        public async Task ThenThePaymentSourceComponentWillNotGenerateAnyContractTypeBalancingTTCoinvestedPayments(ContractType expectedContractType, byte expectedOnProgrammeType)
        {
            await WaitForIt(() =>
            {
                return !FundingSourcePaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                    expectedContractType == receivedEvent.ContractType
                    && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && (TransactionType)expectedOnProgrammeType == receivedEvent.TransactionType
                );
            }, "Failed to find all the funding source payment events");
        }

        [Then(@"the payment source component will not generate any contract type (.*) Completion \(TT(.*)\) coinvested payments")]
        public async Task ThenThePaymentSourceComponentWillNotGenerateAnyContractTypeCompletionTTCoinvestedPayments(ContractType expectedContractType, byte expectedOnProgrammeType)
        {
            await WaitForIt(() =>
            {
                return !FundingSourcePaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                    expectedContractType == receivedEvent.ContractType
                    && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && (TransactionType)expectedOnProgrammeType == receivedEvent.TransactionType
                );
            }, "Failed to find all the funding source payment events");
        }
    }
}
