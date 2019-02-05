using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    public abstract class ApprenticeshipContractTypeMappingTests<TApprenticeshipContractTypeEarningEvent> : EarningEventMappingTests<TApprenticeshipContractTypeEarningEvent>
        where TApprenticeshipContractTypeEarningEvent : ApprenticeshipContractTypeEarningsEvent, new()
    {
        public readonly ContractType contractType;

        public ApprenticeshipContractTypeMappingTests(ContractType contractType)
        {
            this.contractType = contractType;
        }

        protected override TApprenticeshipContractTypeEarningEvent CreatePaymentEvent()
        {
            return new TApprenticeshipContractTypeEarningEvent
            {
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = 1,
                                Amount = 100,
                                PriceEpisodeIdentifier = "pe-1",

                            }
                        }.AsReadOnly()
                    }
                },
                IncentiveEarnings = new List<IncentiveEarning>
                {
                    new IncentiveEarning
                    {
                        Type = IncentiveEarningType.Balancing16To18FrameworkUplift,
                        CensusDate = DateTime.Today,
                        Periods = new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = 1,
                                Amount = 50,
                                PriceEpisodeIdentifier = "pe-1"
                            }
                        }.AsReadOnly()
                    }
                }
            };
        }

        [Test]
        public void Maps_Periods()
        {
            var model = Mapper.Map<EarningEventModel>(PaymentEvent);
            model.Periods.Count.Should().Be(PaymentEvent.OnProgrammeEarnings.SelectMany(x => x.Periods).Count() + PaymentEvent.IncentiveEarnings.SelectMany(x => x.Periods).Count());
        }

        [Test]
        public void Maps_ContractType()
        {
            Mapper.Map<EarningEventModel>(PaymentEvent).ContractType.Should().Be(this.contractType);
        }
    }
}