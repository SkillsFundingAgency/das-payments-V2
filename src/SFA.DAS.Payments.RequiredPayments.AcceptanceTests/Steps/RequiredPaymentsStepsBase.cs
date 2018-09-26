using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public abstract class RequiredPaymentsStepsBase: StepsBase
    {
        public List<OnProgrammePaymentDue> PaymentsDue { get => Get<List<OnProgrammePaymentDue>>(); set => Set(value); }
        protected RequiredPaymentsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
    }
}