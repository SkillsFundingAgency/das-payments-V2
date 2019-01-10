using System;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class EarningSteps: RequiredPaymentsStepsBase
    {
        public EarningSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"the SFA contribution percentage is (.*)%")]
        public void GivenTheSFAContributionPercentageIs(decimal sfaContributionPercentage)
        {
            Console.WriteLine($"Got sfa contribution percentage: {sfaContributionPercentage}");
            SfaContributionPercentage = sfaContributionPercentage;
        }


        [Given(@"the following contract type (.*) on programme earnings for periods (.*)-(.*) are provided in the latest ILR for the academic year (.*):")]
        public void GivenTheFollowingContractTypeOnProgrammeEarningsForPeriods(short contractType, byte fromPeriod, byte toPeriod, string academicYear, Table table)
        {
            var rawEarnings = table.CreateSet<ContractTypeEarning>();

            var earning = rawEarnings.FirstOrDefault();

            earning.FromPeriod = fromPeriod;
            earning.ToPeriod = toPeriod;
            earning.AcademicYear = academicYear;

            Context[$"ContractType{contractType}OnProgrammeEarningsLearning"] = earning;
        }

        //[Given(@"the following contract type (.*) on programme earnings for period (.*) are provided in the latest ILR for the academic year (.*):")]
        //public void GivenTheFollowingContractTypeOnProgrammeEarningsForPeriod(short contractType, byte period, string academicYear, Table table)
        //{
        //    var rawEarnings = table.CreateSet<ContractTypeEarning>();

        //    var earning = rawEarnings.FirstOrDefault();

        //    earning.FromPeriod = period;
        //    earning.ToPeriod = period;
        //    earning.AcademicYear = academicYear;
        //    context[$"ContractType{contractType}OnProgrammeEarningsCompletion"] = earning;
        //}


        [Given(@"the following contract type (.*) incentive earnings for periods (.*)-(.*) are provided in the latest ILR for the academic year (.*):")]
        public void GivenTheFollowingContractTypeIncentiveEarningsForPeriods(short contractType, short fromPeriod, short toPeriod, string academicYear, Table table)
        {
            var rawEarnings = table.CreateSet<IncentiveEarning>();

            // var incentiveEarnings = new ContractTypeEarning(contractType, fromPeriod, toPeriod, academicYear, rawEarnings.ToList());

            //context[$"ContractType{contractType}IncentiveEarnings"] = incentiveEarnings;
        }

        [Given(@"the following contract type (.*) incentive earnings for period (.*):")]
        public void GivenTheFollowingContractTypeIncentiveEarningsForPeriod(int contractType, int period, Table table)
        {
        }


        [Given(@"the following completion earnings:")]
        public void GivenTheFollowingCompletionEarnings(Table table)
        {
        }

        [Given(@"the following completion earnings for period (.*):")]
        public void GivenTheFollowingCompletionEarnings(int period, Table table)
        {
        }

        [Given(@"the earning events component generates the following contract type (.*) earnings:")]
        public void GivenTheEarningEventsComponentGeneratesTheFollowingContractTypeEarningEvents(byte contractType, Table payments)
        {
            ContractType = contractType;
            Earnings = payments.CreateSet<OnProgrammeEarning>()
                .ToList();
        }

        [Given(@"the earning events component generates more earnings:")]
        public void GivenTheEarningEventsComponentGeneratesMoreEarningEvents(Table payments)
        {
            Earnings.AddRange(payments.CreateSet<OnProgrammeEarning>().ToList());
        }
    }
}
