using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    public abstract class FunctionalSkillMappingTests<T>: EarningEventMappingTests<T> where T: FunctionalSkillEarningsEvent, new()
    {
        protected override T CreatePaymentEvent()
        {
            return new T
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


        [Test]
        public void MapsContractType()
        {
            Mapper.Map<EarningEventModel>(PaymentEvent).ContractType.Should().Be(PaymentEvent.ContractType);
        }

    }
}