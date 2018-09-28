using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class EarningSteps: PaymentsDueStepsBase
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

        [Given(@"the following contract type (.*) On Programme earnings are provided in the latest ILR for the current academic year:")]
        public void GivenTheFollowingContractTypeOnProgrammeEarningsAreProvidedInTheLatestILRForTheCurrentAcademicYear(int p0, Table table)
        {
            var rawEarnings = table.CreateSet<ContractTypeEarning>().ToArray();
            var transactionType = rawEarnings[0].TransactionType;

            this.Act2EarningEvents = new List<ApprenticeshipContractType2EarningEvent>
            {
                new ApprenticeshipContractType2EarningEvent
                {
                    OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new []
                    {
                        new OnProgrammeEarning
                        {
                            Type = transactionType,
                            Periods = new ReadOnlyCollection<EarningPeriod>(rawEarnings.Select(e => new EarningPeriod
                            {
                                Period = new CalendarPeriod(this.CollectionYear, e.Delivery_Period),
                                Amount = e.Amount,
                                PriceEpisodeIdentifier = e.PriceEpisodeIdentifier
                            }).ToList())
                        }
                    })
                }
            };

            //yesScenarioCtx[$"ContractType{this.ContractType}OnProgrammeEarningsLearning"] = earning;
        }

        //[Given(@"the following contract type (.*) on programme earnings for periods (.*)-(.*) are provided in the latest ILR for the academic year (.*):")]
        //public void GivenTheFollowingContractTypeOnProgrammeEarningsForPeriods(short contractType, byte fromPeriod, byte toPeriod, string academicYear, Table table)
        //{
        //    var rawEarnings = table.CreateSet<ContractTypeEarning>();

        //    var earning = rawEarnings.FirstOrDefault();

        //    earning.FromPeriod = fromPeriod;
        //    earning.ToPeriod = toPeriod;
        //    earning.AcademicYear = academicYear;

        //    ScenarioCtx[$"ContractType{contractType}OnProgrammeEarningsLearning"] = earning;
        //}

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


        //[Given(@"the following contract type (.*) incentive earnings for periods (.*)-(.*) are provided in the latest ILR for the academic year (.*):")]
        //public void GivenTheFollowingContractTypeIncentiveEarningsForPeriods(short contractType, short fromPeriod, short toPeriod, string academicYear, Table table)
        //{
        //    var rawEarnings = table.CreateSet<IncentiveEarning>();

        //    // var incentiveEarnings = new ContractTypeEarning(contractType, fromPeriod, toPeriod, academicYear, rawEarnings.ToList());

        //    //context[$"ContractType{contractType}IncentiveEarnings"] = incentiveEarnings;
        //}

        //[Given(@"the following contract type (.*) incentive earnings for period (.*):")]
        //public void GivenTheFollowingContractTypeIncentiveEarningsForPeriod(int contractType, int period, Table table)
        //{
        //}


        //[Given(@"the following completion earnings:")]
        //public void GivenTheFollowingCompletionEarnings(Table table)
        //{
        //}

        //[Given(@"the following completion earnings for period (.*):")]
        //public void GivenTheFollowingCompletionEarnings(int period, Table table)
        //{
        //}
    }
}
