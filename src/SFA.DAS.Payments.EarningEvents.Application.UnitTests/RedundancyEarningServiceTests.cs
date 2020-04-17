using System;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Application.Services;
using SFA.DAS.Payments.EarningEvents.Application.UnitTests.Builders;
using SFA.DAS.Payments.EarningEvents.Application.UnitTests.Helpers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class RedundancyEarningServiceTests
    {
        private IPaymentLogger logger;
        private IMapper mapper;
        private RedundancyEarningService service;

        [SetUp]
        public void Setup()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EarningsEventProfile>()));
            service = new RedundancyEarningService(new RedundancyEarningEventFactory(mapper));
        }


        [Test]
        public void SplitContractEarningByRedundancyDate_ShouldCreateSplitOriginalEarningAtCorrectPeriodAndRemoveCorrectPeriods()
        {
            var act1Builder = new ContractTypeEarningEventBuilder<ApprenticeshipContractType1EarningEvent>();
            var act1Earning = act1Builder.Build();
            ValidateEarningSplitAndType(act1Earning, typeof(ApprenticeshipContractType1RedundancyEarningEvent));

             var act2Builder = new ContractTypeEarningEventBuilder<ApprenticeshipContractType2EarningEvent>();
            var act2Earning = act2Builder.Build();
            ValidateEarningSplitAndType(act2Earning, typeof(ApprenticeshipContractType2RedundancyEarningEvent));
        }

        private void ValidateEarningSplitAndType(ApprenticeshipContractTypeEarningsEvent earning, Type expectedRedundancyType
            )
        {
            
            var redundancyDate = new DateTime(1920, 5, 1);
            var redundancyPeriod = redundancyDate.GetCollectionPeriodFromDate();

            earning.OnProgrammeEarnings.ForEach(ope => { ope.Periods.Should().HaveCount(12); });
            earning.IncentiveEarnings.ForEach(ie => { ie.Periods.Should().HaveCount(12); });


            var events = service.SplitContractEarningByRedundancyDate(earning, redundancyDate);
            events.Should().HaveCount(2);
            var originalEarningEvent = events[0];
            originalEarningEvent.Should().BeOfType(earning.GetType());
            originalEarningEvent.OnProgrammeEarnings.Should().HaveCount(3);

            var expectedOriginalCount = redundancyPeriod - 1;

            originalEarningEvent.OnProgrammeEarnings.ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedOriginalCount);
                Assert.IsTrue(!ope.Periods.ToList().Any(p => p.Period >= redundancyPeriod));
                Assert.IsTrue(ope.Periods.ToList().All(p => p.Period < redundancyPeriod));
            });
            originalEarningEvent.IncentiveEarnings.ForEach(ie =>
            {
                ie.Periods.Should().HaveCount(expectedOriginalCount);
                Assert.IsTrue(!ie.Periods.ToList().Any(p => p.Period >= redundancyPeriod));
                Assert.IsTrue(ie.Periods.ToList().All(p => p.Period < redundancyPeriod));
            });

            var expectedRedundancyPeriod = 12 - redundancyPeriod + 1;

            var redundancyEvent = events[1];
            redundancyEvent.Should().BeOfType(expectedRedundancyType);
            redundancyEvent.OnProgrammeEarnings.Should().HaveCount(3);
            redundancyEvent.OnProgrammeEarnings.ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedRedundancyPeriod);
                Assert.IsTrue(!ope.Periods.ToList().Any(p => p.Period < redundancyPeriod));
                Assert.IsTrue(ope.Periods.ToList().All(p => p.Period >= redundancyPeriod));
                Assert.IsTrue(ope.Periods.ToList().All(p => p.SfaContributionPercentage == 1m));
            });
            redundancyEvent.IncentiveEarnings.ForEach(ie =>
            {
                ie.Periods.Should().HaveCount(expectedRedundancyPeriod);
                Assert.IsTrue(!ie.Periods.ToList().Any(p => p.Period < redundancyPeriod));
                Assert.IsTrue(ie.Periods.ToList().All(p => p.Period >= redundancyPeriod));
            });
        }
    }
}