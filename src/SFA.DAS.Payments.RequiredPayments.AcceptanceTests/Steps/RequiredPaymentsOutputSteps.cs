using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Application;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsOutputSteps: StepsBase
    {
        private readonly ScenarioContext context;
        private readonly LearnRefNumberGenerator generator;


        public RequiredPaymentsOutputSteps(ScenarioContext context, LearnRefNumberGenerator generator)
        {
            this.context = context;
            this.generator = generator;

        }
        [Then(@"the payments due component will generate the following payable earnings:")]
        public void ThenThePaymentsDueComponentWillGenerateTheFollowingPayableEarnings(Table table)
        {
        }

        [Then(@"the payments due component will generate the following contract type (.*) payable earnings:")]
        public void ThenThePaymentsDueComponentWillGenerateTheFollowingPayableEarnings(short contractType, Table table)
        {
            WaitForIt(() =>
            {
                var results = ApprenticeshipContractType2Handler.ReceivedEvents;

                if (results == null || !results.Any())
                {
                    return false;
                }

                var payableEarnings = table.CreateSet<PayableEarning>().ToList();

                return payableEarnings.All(x => results.Any(resultEvent => x.Period == resultEvent.DeliveryPeriod.Period
                                                                    && x.Ukprn == resultEvent.Ukprn
                                                                    && generator.Generate(x.Ukprn, x.LearnRefNumber)
                                                                        .ToString() ==
                                                                    resultEvent.Learner.ReferenceNumber
                                                                    && x.OnProgrammeEarningType.HasValue
                                                                    && x.OnProgrammeEarningType.Value ==
                                                                    resultEvent.OnProgrammeEarningType
                                                                    && x.Amount == resultEvent.AmountDue));
            }, "Failed to find all the required payment earning events");
        }
    }
}