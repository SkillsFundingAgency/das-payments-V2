using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Application;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentsDueOutputSteps: StepsBase
    {
        private readonly ScenarioContext context;
        private readonly LearnRefNumberGenerator generator;

        //protected ApprenticeshipContractType2EarningEvent EarningEvent
        //{
        //    get => Get<ApprenticeshipContractType2EarningEvent>();
        //    set => Set(value);
        //}

        public PaymentsDueOutputSteps(ScenarioContext context, LearnRefNumberGenerator generator)
        {
            this.context = context;
            this.generator = generator;

   //         EarningEvent = context.Get<ApprenticeshipContractType2EarningEvent>();
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

                return payableEarnings.All(x => results.Any(resultEvent => x.Period == resultEvent.Period
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