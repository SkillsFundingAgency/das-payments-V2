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
        public void SplitFunctionSkillEarningByRedundancyDate_ShouldCreateSplitOriginalEarningAtCorrectPeriodAndRemoveCorrectPeriods()
        {
            var act1FandSBuilder = new Builders.TestFunctionalSkillsEarningEventBuilder<Act1FunctionalSkillEarningsEvent>();
            var act1FandSEarning = act1FandSBuilder.Build();

            ValidateFunctionalSkillEarningsSplitAndType(act1FandSEarning, typeof(Act1RedundancyFunctionalSkillEarningsEvent));


            var act2FandSBuilder = new Builders.TestFunctionalSkillsEarningEventBuilder<Act2FunctionalSkillEarningsEvent>();
            var act2FandSEarning = act2FandSBuilder.Build();

            ValidateFunctionalSkillEarningsSplitAndType(act2FandSEarning, typeof(Act2RedundancyFunctionalSkillEarningsEvent));
        }

        private void ValidateFunctionalSkillEarningsSplitAndType(FunctionalSkillEarningsEvent act1FandSEarning, Type expectedRedundancyType)
        {
            var redundancyDate = new DateTime(1920, 7, 1);
            var redundancyPeriod = redundancyDate.GetCollectionPeriodFromDate();
            act1FandSEarning.Earnings.ToList().ForEach(fse => fse.Periods.Should().HaveCount(12));

            var events = service.SplitFunctionSkillEarningByRedundancyDate(act1FandSEarning, redundancyDate);
            events.Should().HaveCount(2);
            var originalEvent = events[0];

            originalEvent.Should().BeOfType(act1FandSEarning.GetType());
            originalEvent.Earnings.Should().HaveCount(3);

            var expectedOriginalCount = redundancyPeriod - 1;

            originalEvent.Earnings.ToList().ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedOriginalCount);
                Assert.IsTrue(!ope.Periods.ToList().Any(p => p.Period >= redundancyPeriod));
                Assert.IsTrue(ope.Periods.ToList().All(p => p.Period < redundancyPeriod));
            });

            var redundancyEvent = events[1];

            var expectedRedundancyPeriod = 12 - redundancyPeriod + 1;
            redundancyEvent.Should().BeOfType(expectedRedundancyType);
            redundancyEvent.Earnings.Should().HaveCount(3);
            redundancyEvent.Earnings.ToList().ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedRedundancyPeriod);
                Assert.IsTrue(!ope.Periods.ToList().Any(p => p.Period < redundancyPeriod));
                Assert.IsTrue(ope.Periods.ToList().All(p => p.Period >= redundancyPeriod));
                Assert.IsTrue(ope.Periods.ToList().All(p => p.SfaContributionPercentage == 1m));
            });
        }

        [Test]
        public void SplitContractEarningByRedundancyDate_ShouldCreateSplitOriginalEarningAtCorrectPeriodAndRemoveCorrectPeriods()
        {
            var act1Builder = new TestContractTypeEarningEventBuilder<ApprenticeshipContractType1EarningEvent>();
            var act1Earning = act1Builder.Build();
            ValidateContractTypeEarningsSplitAndType(act1Earning, typeof(ApprenticeshipContractType1RedundancyEarningEvent));

             var act2Builder = new TestContractTypeEarningEventBuilder<ApprenticeshipContractType2EarningEvent>();
            var act2Earning = act2Builder.Build();
            ValidateContractTypeEarningsSplitAndType(act2Earning, typeof(ApprenticeshipContractType2RedundancyEarningEvent));
        }

        private void ValidateContractTypeEarningsSplitAndType(ApprenticeshipContractTypeEarningsEvent earning, Type expectedRedundancyType
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