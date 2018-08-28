﻿using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.Tests.Core.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentsDueInputSteps
    {
        private readonly ScenarioContext scenarioContext;

        public PaymentsDueInputSteps(ScenarioContext context)
        {
            scenarioContext = context;
        }

        [When(@"a TOBY is received")]
        public void WhenATobyIsReceived()
        {
            // Get all the input data
            var processingPeriod = (short)scenarioContext["ProcessingPeriod"];

            var learners = scenarioContext["Learners"] as IEnumerable<Learner>;

            IEnumerable<Course> courses;

            if (scenarioContext.ContainsKey("Courses"))
            {
                courses = scenarioContext["Courses"] as IEnumerable<Course>;
            }

            ContractTypeEarnings contractType1OnProgrammeEarnings;

            if (scenarioContext.ContainsKey("ContractType1OnProgrammeEarnings"))
            {
                contractType1OnProgrammeEarnings = scenarioContext["ContractType1OnProgrammeEarnings"] as ContractTypeEarnings;
            }

            ContractTypeEarnings contractType2OnProgrammeEarnings;

            if (scenarioContext.ContainsKey("ContractType2OnProgrammeEarnings"))
            {
                contractType2OnProgrammeEarnings = scenarioContext["ContractType2OnProgrammeEarnings"] as ContractTypeEarnings;
            }

            // now create an event from all this stuff and sent it off
        }
    }
}