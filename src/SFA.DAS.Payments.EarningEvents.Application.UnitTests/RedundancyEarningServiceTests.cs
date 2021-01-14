using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Application.Services;
using SFA.DAS.Payments.EarningEvents.Application.UnitTests.Builders;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class RedundancyEarningServiceTests
    {
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
            var act1FsBuilder = new TestFunctionalSkillsEarningEventBuilder<Act1FunctionalSkillEarningsEvent>();
            var act1FsEarning = act1FsBuilder.Build();

            ValidateFunctionalSkillEarningsSplitAndType(act1FsEarning, typeof(Act1RedundancyFunctionalSkillEarningsEvent));


            var act2FsBuilder = new TestFunctionalSkillsEarningEventBuilder<Act2FunctionalSkillEarningsEvent>();
            var act2FsEarning = act2FsBuilder.Build();

            ValidateFunctionalSkillEarningsSplitAndType(act2FsEarning, typeof(Act2RedundancyFunctionalSkillEarningsEvent));
        }

        private void ValidateFunctionalSkillEarningsSplitAndType(FunctionalSkillEarningsEvent act1FsEarning, Type expectedRedundancyType)
        {
            var redundancyDate = new DateTime(1920, 7, 1);
            var redundancyPeriod =redundancyDate.GetPeriodFromDate();
            act1FsEarning.Earnings.ToList().ForEach(fse => fse.Periods.Should().HaveCount(12));

            var events = service.OriginalAndRedundancyFunctionalSkillEarningEventIfRequired(act1FsEarning, new List<byte>{ 12 });
            events.Should().HaveCount(2);
            var originalEvent = events[0];

            originalEvent.Should().BeOfType(act1FsEarning.GetType());

            var expectedOriginalCount = redundancyPeriod - 1;

            originalEvent.Earnings.ToList().ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedOriginalCount);
                Assert.IsTrue(ope.Periods.ToList().All(p => p.Period < redundancyPeriod));
            });

            var redundancyEvent = events[1];

            var expectedRedundancyPeriod = 12 - redundancyPeriod + 1;
            redundancyEvent.Should().BeOfType(expectedRedundancyType);
            redundancyEvent.Earnings.ToList().ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedRedundancyPeriod);
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

        [Test]
        public void
            SplitContractEarningByRedundancyDate_GiveRedundancyDateOutsideLearningPeriod_LeaveEarningEventUnaffected()
        {
            var act1Builder = new TestContractTypeEarningEventBuilder<ApprenticeshipContractType1EarningEvent>();
            var act1Earning = act1Builder.Build();

            var redundancyDate = new DateTime(1921, 5, 1);

            var events = service.OriginalAndRedundancyEarningEventIfRequired(act1Earning, new List<byte>{10, 11, 12});
            events.Should().HaveCount(2);
        }

        private void ValidateContractTypeEarningsSplitAndType(ApprenticeshipContractTypeEarningsEvent earning, Type expectedRedundancyType
            )
        {
            
            var redundancyDate = new DateTime(1920, 5, 1);
            var redundancyPeriod = redundancyDate.GetPeriodFromDate();

            earning.OnProgrammeEarnings.ForEach(ope => { ope.Periods.Should().HaveCount(12); });
            earning.IncentiveEarnings.ForEach(ie => { ie.Periods.Should().HaveCount(12); });


            var events = service.OriginalAndRedundancyEarningEventIfRequired(earning, new List<byte> { 10, 11, 12 });
            events.Should().HaveCount(2);
            var originalEarningEvent = events[0];
            originalEarningEvent.Should().BeOfType(earning.GetType());
            originalEarningEvent.OnProgrammeEarnings.Should().HaveCount(3);

            var expectedOriginalCount = redundancyPeriod - 1;

            originalEarningEvent.OnProgrammeEarnings.ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedOriginalCount);
                Assert.IsTrue(ope.Periods.ToList().All(p => p.Period < redundancyPeriod));
            });
            originalEarningEvent.IncentiveEarnings.ForEach(ie =>
            {
                ie.Periods.Should().HaveCount(expectedOriginalCount);
                Assert.IsTrue(ie.Periods.ToList().All(p => p.Period < redundancyPeriod));
            });

            var expectedRedundancyPeriod = 12 - redundancyPeriod + 1;

            var redundancyEvent = events[1];
            redundancyEvent.Should().BeOfType(expectedRedundancyType);
            redundancyEvent.OnProgrammeEarnings.Should().HaveCount(3);
            redundancyEvent.OnProgrammeEarnings.ForEach(ope =>
            {
                ope.Periods.Should().HaveCount(expectedRedundancyPeriod);
                Assert.IsTrue(ope.Periods.ToList().All(p => p.Period >= redundancyPeriod));
                Assert.IsTrue(ope.Periods.ToList().All(p => p.SfaContributionPercentage == 1m));
            });
            redundancyEvent.IncentiveEarnings.ForEach(ie =>
            {
                ie.Periods.Should().HaveCount(expectedRedundancyPeriod);
                Assert.IsTrue(ie.Periods.ToList().All(p => p.Period >= redundancyPeriod));
            });
        }
    }
}