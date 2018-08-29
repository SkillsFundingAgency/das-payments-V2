using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentSource.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentSourceInputSteps
    {
        [When(@"MASH is received")]
        public void WhenMashIsReceived()
        {
            var sessionId = (Guid)ScenarioContext.Current["SessionId"];
            // Take the input data and create a RequiredPayment message for all the input payable earnings
        }
    }

    
}