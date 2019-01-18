using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    public class FunctionalSkillMappingTests: EarningEventMappingTests<FunctionalSkillEarningsEvent>
    {
        protected override FunctionalSkillEarningsEvent CreatePaymentEvent()
        {
            return new FunctionalSkillEarningsEvent
            {
                Earnings = new List<FunctionalSkillEarning>
                {
                    new FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.BalancingMathsAndEnglish,
                        Periods = new List<EarningPeriod>
                        {
                            new EarningPeriod{ Period = 1, Amount = 100, PriceEpisodeIdentifier = "pe-1"}
                        }.AsReadOnly()
                    },
                }.AsReadOnly()
            };
        }

        [Test]
        public void Maps_Earnings()
        {
            var model = Mapper.Map<EarningEventModel>(PaymentEvent);
            model.Periods.Count.Should().Be(PaymentEvent.Earnings.SelectMany(x => x.Periods).Count());
        }


        [TestCaseSource(nameof(GetContractTypes))]
        public void Maps_ContractType(ContractType contractType)
        {
            Mapper.Map<EarningEventModel>(PaymentEvent).ContractType.Should().Be(ContractType.Act2);
        }

    }
}