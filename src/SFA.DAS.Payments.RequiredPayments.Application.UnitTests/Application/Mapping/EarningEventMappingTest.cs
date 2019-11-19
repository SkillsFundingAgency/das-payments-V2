﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Mapping
{
    [TestFixture]
    public class EarningEventMappingTest
    {
        public EarningEventMappingTest()
        {
            ukprn = 123L;
            uln = 456L;
        }

        private IMapper mapper;
        private static long ukprn;
        private static long uln;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [Test]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(CalculatedRequiredIncentiveAmount))]
        [TestCase(typeof(CalculatedRequiredLevyAmount))]
        public void AmountIsCorrect(Type requiredPaymentEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            var expectedAmount = 100;

            var requiredPayment = new RequiredPayment
            {
                Amount = expectedAmount,
            };

            var actual = mapper.Map(requiredPayment, requiredPaymentEvent);
            actual.AmountDue.Should().Be(expectedAmount);
        }

        [Test]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(CalculatedRequiredIncentiveAmount))]
        [TestCase(typeof(CalculatedRequiredLevyAmount))]
        public void PriceEpisodeIdentifierIsCorrect(Type requiredPaymentEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            var expectedPriceEpisodeIdentifier = "peid";

            var requiredPayment = new RequiredPayment
            {
                PriceEpisodeIdentifier = expectedPriceEpisodeIdentifier,
            };

            var actual = mapper.Map(requiredPayment, requiredPaymentEvent);
            actual.PriceEpisodeIdentifier.Should().Be(expectedPriceEpisodeIdentifier);
        }

        [Test]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(CalculatedRequiredLevyAmount))]
        public void SfaPercentageIsCorrect(Type requiredPaymentEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as CalculatedRequiredOnProgrammeAmount;
            var expectedSfaPercentage = 100;

            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = expectedSfaPercentage,
            };

            var actual = mapper.Map(requiredPayment, requiredPaymentEvent);
            actual.SfaContributionPercentage.Should().Be(expectedSfaPercentage);
        }

        [Test]
        [TestCase(typeof(PayableEarningEvent), typeof(CalculatedRequiredIncentiveAmount))]
        [TestCase(typeof(PayableEarningEvent), typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(PayableEarningEvent), typeof(CalculatedRequiredLevyAmount))]
        [TestCase(typeof(PayableEarningEvent), typeof(CompletionPaymentHeldBackEvent))]
        public void ContractTypeIsCorrectForPayableEarningEvent(Type earningEventType, Type requiredPaymentEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            var earningEvent = Activator.CreateInstance(earningEventType);

            var actual = mapper.Map(earningEvent, requiredPaymentEvent);
            actual.ContractType.Should().Be(ContractType.Act1);
        }

        [Test]
        [TestCase(typeof(ApprenticeshipContractType2EarningEvent), typeof(CalculatedRequiredIncentiveAmount))]
        [TestCase(typeof(ApprenticeshipContractType2EarningEvent), typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(ApprenticeshipContractType2EarningEvent), typeof(CalculatedRequiredLevyAmount))]
        [TestCase(typeof(ApprenticeshipContractType2EarningEvent), typeof(CompletionPaymentHeldBackEvent))]
        public void ContractTypeIsCorrectForNotLevyEvent(Type earningEventType, Type requiredPaymentEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            var earningEvent = Activator.CreateInstance(earningEventType);

            var actual = mapper.Map(earningEvent, requiredPaymentEvent);
            actual.ContractType.Should().Be(ContractType.Act2);
        }


        [Test]
        [TestCase( typeof(CalculatedRequiredIncentiveAmount))]
        public void MathsAndEnglishRequiredMappingShouldMapEarningEventIdCorrectly(Type requiredPaymentEventType)
        {
            var earningEventId = Guid.NewGuid();
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            var earningEvent = new PayableFunctionalSkillEarningEvent {EarningEventId = earningEventId};
            var actual = mapper.Map(earningEvent, requiredPaymentEvent);
            actual.EarningEventId.Should().Be(earningEventId);
        }


        [Test]
        [TestCase(typeof(CalculatedRequiredIncentiveAmount), ContractType.Act1)]
        [TestCase(typeof(CalculatedRequiredIncentiveAmount), ContractType.Act2)]
        public void ContractTypeIsCorrectForFunctionalSkills(Type requiredPaymentEventType, ContractType expectedContractType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;
            requiredPaymentEvent.ContractType = expectedContractType;

            IFunctionalSkillEarningEvent earningEvent = null;

            switch (expectedContractType)
            {
                case ContractType.Act1:
                    earningEvent = new PayableFunctionalSkillEarningEvent();
                    break;
                case ContractType.Act2:
                    earningEvent = new Act2FunctionalSkillEarningsEvent();
                    break;
            }

            var actual = mapper.Map(earningEvent, requiredPaymentEvent);
            actual.ContractType.Should().Be(expectedContractType);
        }

        [Test]
        public void TestPayableEarningEventMap()
        {
            // arrange
            var payableEarning = CreatePayableEarning();
            PeriodisedRequiredPaymentEvent requiredPayment = new CalculatedRequiredLevyAmount
            {
                SfaContributionPercentage = .9m,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion
            };

            // act
            mapper.Map(payableEarning, requiredPayment);

            // assert
            AssertCommonProperties(requiredPayment, payableEarning);

            var act1RequiredPayment = (CalculatedRequiredLevyAmount)requiredPayment;

            Assert.AreEqual(payableEarning.AgreementId, act1RequiredPayment.AgreementId);
            Assert.AreEqual(.9m, act1RequiredPayment.SfaContributionPercentage);
            Assert.AreEqual(OnProgrammeEarningType.Completion, act1RequiredPayment.OnProgrammeEarningType);
            Assert.AreEqual(payableEarning.EarningEventId, act1RequiredPayment.EarningEventId);
            Assert.AreNotEqual(payableEarning.EventId, act1RequiredPayment.EarningEventId);
        }

        [Test]
        public void TestEarningPeriodMap()
        {
            var earningPeriod = new EarningPeriod
            {
                ApprenticeshipId = 101,
                ApprenticeshipPriceEpisodeId = 102,
                Period = 1,
                AccountId = 103,
                Priority = 104,
                SfaContributionPercentage = 1m,
                PriceEpisodeIdentifier = "123-01",
                Amount = 1000000,
                AgreedOnDate = DateTime.Today
            };
            var requiredPayment = new CalculatedRequiredLevyAmount
            {
                SfaContributionPercentage = .9m,
                OnProgrammeEarningType = OnProgrammeEarningType.Balancing
            };

            // act
            mapper.Map(earningPeriod, requiredPayment);

            var act1RequiredPayment = requiredPayment;

            Assert.AreEqual(earningPeriod.Period, act1RequiredPayment.DeliveryPeriod);
            Assert.AreEqual(earningPeriod.ApprenticeshipId, act1RequiredPayment.ApprenticeshipId);
            Assert.AreEqual(earningPeriod.ApprenticeshipPriceEpisodeId, act1RequiredPayment.ApprenticeshipPriceEpisodeId);
            Assert.AreEqual(earningPeriod.AgreedOnDate, act1RequiredPayment.AgreedOnDate);
            Assert.AreEqual(earningPeriod.Priority, act1RequiredPayment.Priority);
            Assert.AreEqual(earningPeriod.SfaContributionPercentage, act1RequiredPayment.SfaContributionPercentage);
            Assert.AreEqual(OnProgrammeEarningType.Balancing, act1RequiredPayment.OnProgrammeEarningType);

        }

        [Test]
        public void TestEarningPeriodToCalculatedRequiredIncentiveAmountMapping()
        {
            var earningPeriod = new EarningPeriod
            {
                ApprenticeshipId = 101,
                ApprenticeshipPriceEpisodeId = 102,
                Period = 1,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy
            };
            var requiredPayment = new CalculatedRequiredIncentiveAmount
            {
                Type = IncentivePaymentType.OnProgrammeMathsAndEnglish
            };

            // act
            mapper.Map(earningPeriod, requiredPayment);
            var mathsAndEnglishPayment = requiredPayment;
            Assert.AreEqual(earningPeriod.Period, mathsAndEnglishPayment.DeliveryPeriod);
            Assert.AreEqual(earningPeriod.ApprenticeshipId, mathsAndEnglishPayment.ApprenticeshipId);
            Assert.AreEqual(earningPeriod.ApprenticeshipPriceEpisodeId,
                mathsAndEnglishPayment.ApprenticeshipPriceEpisodeId);
            Assert.AreEqual(earningPeriod.ApprenticeshipEmployerType,
                mathsAndEnglishPayment.ApprenticeshipEmployerType);
            Assert.AreEqual(IncentivePaymentType.OnProgrammeMathsAndEnglish, mathsAndEnglishPayment.Type);
        }



        [Test]
        [TestCaseSource(nameof(GetFunctionalSkillEarningEvents))]
        public void TestFunctionalSkillEarningEventMap(IFunctionalSkillEarningEvent functionalSkillEarningsEvent)
        {
            // arrange
            PeriodisedRequiredPaymentEvent requiredPayment = new CalculatedRequiredIncentiveAmount
            {
                Ukprn = ukprn,
                Learner = new Learner { ReferenceNumber = "R", Uln = uln },
                StartDate = DateTime.Today.AddDays(-10)
            };

            // act
            mapper.Map(functionalSkillEarningsEvent, requiredPayment);

            // assert
            AssertCommonProperties(requiredPayment, functionalSkillEarningsEvent);
            Assert.AreEqual(requiredPayment.LearningAim.FundingLineType, functionalSkillEarningsEvent.LearningAim.FundingLineType);
            functionalSkillEarningsEvent.StartDate.Should().Be(requiredPayment.StartDate);
        }

        [Test]
        [TestCase(typeof(CalculatedRequiredIncentiveAmount))]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount))]
        [TestCase(typeof(CalculatedRequiredLevyAmount))]
        public void PriceEpisodeMapsEarningsInfo(Type requiredPaymentEventType)
        {
            var priceEpisode = new PriceEpisode
            {
                EffectiveTotalNegotiatedPriceStartDate = DateTime.UtcNow,
                PlannedEndDate = DateTime.UtcNow,
                ActualEndDate = DateTime.UtcNow,
                CompletionAmount = 100M,
                InstalmentAmount = 200M,
                NumberOfInstalments = 16,
            };

            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as PeriodisedRequiredPaymentEvent;

            mapper.Map(priceEpisode, requiredPaymentEvent);
            requiredPaymentEvent.StartDate.Should().Be(priceEpisode.EffectiveTotalNegotiatedPriceStartDate);
            requiredPaymentEvent.PlannedEndDate.Should().Be(priceEpisode.PlannedEndDate);
            requiredPaymentEvent.ActualEndDate.Should().Be(priceEpisode.ActualEndDate);
            requiredPaymentEvent.CompletionStatus.Should().Be(0);
            requiredPaymentEvent.CompletionAmount.Should().Be(priceEpisode.CompletionAmount);
            requiredPaymentEvent.InstalmentAmount.Should().Be(priceEpisode.InstalmentAmount);
            requiredPaymentEvent.NumberOfInstalments.Should().Be((short)priceEpisode.NumberOfInstalments);
        }

        [Test]
        public void MapPriceEpisodeToLearningAimFundingLineType()
        {
            var priceEpisode = new PriceEpisode
            {
                EffectiveTotalNegotiatedPriceStartDate = DateTime.UtcNow,
                PlannedEndDate = DateTime.UtcNow,
                ActualEndDate = DateTime.UtcNow,
                CompletionAmount = 100M,
                InstalmentAmount = 200M,
                NumberOfInstalments = 16,
                FundingLineType = "19+ Apprenticeship Non Levy Contract (procured)"
            };

            var learningAim = new LearningAim
            {
                FundingLineType = "flt",
                PathwayCode = 3,
                StandardCode = 4,
                ProgrammeType = 5,
                FrameworkCode = 6,
                Reference = "7"
            };

            mapper.Map(priceEpisode, learningAim);

            learningAim.FundingLineType.Should().Be(priceEpisode.FundingLineType);
        }

        [Test]
        public void PaymentHistoryEntityMapsEarningsInfo()
        {
            var paymentHistoryEntity = new PaymentHistoryEntity
            {
                CollectionPeriod = new CollectionPeriod(),
                StartDate = DateTime.UtcNow,
                PlannedEndDate = DateTime.UtcNow,
                ActualEndDate = DateTime.UtcNow,
                CompletionAmount = 100M,
                InstalmentAmount = 200M,
                NumberOfInstalments = 16
            };

            var payment = mapper.Map<Payment>(paymentHistoryEntity);

            payment.StartDate.Should().Be(paymentHistoryEntity.StartDate);
            payment.PlannedEndDate.Should().Be(paymentHistoryEntity.PlannedEndDate);
            payment.ActualEndDate.Should().Be(paymentHistoryEntity.ActualEndDate);
            payment.CompletionStatus.Should().Be(0);
            payment.CompletionAmount.Should().Be(paymentHistoryEntity.CompletionAmount);
            payment.InstalmentAmount.Should().Be(paymentHistoryEntity.InstalmentAmount);
            payment.NumberOfInstalments.Should().Be(paymentHistoryEntity.NumberOfInstalments);
        }

        private static void AssertCommonProperties(PeriodisedRequiredPaymentEvent requiredPayment, IEarningEvent earning)
        {
            Assert.AreEqual(requiredPayment.Learner.Uln, earning.Learner.Uln);
            Assert.AreEqual(requiredPayment.Learner.ReferenceNumber, earning.Learner.ReferenceNumber);
            Assert.AreEqual(requiredPayment.Ukprn, earning.Ukprn);
            Assert.AreEqual(requiredPayment.CollectionPeriod.Period, earning.CollectionPeriod.Period);
            Assert.AreEqual(requiredPayment.CollectionPeriod.AcademicYear, earning.CollectionPeriod.AcademicYear);
            Assert.AreEqual(requiredPayment.LearningAim.PathwayCode, earning.LearningAim.PathwayCode);
            Assert.AreEqual(requiredPayment.LearningAim.FrameworkCode, earning.LearningAim.FrameworkCode);
            Assert.AreEqual(requiredPayment.LearningAim.ProgrammeType, earning.LearningAim.ProgrammeType);
            Assert.AreEqual(requiredPayment.LearningAim.Reference, earning.LearningAim.Reference);
            Assert.AreEqual(requiredPayment.LearningAim.StandardCode, earning.LearningAim.StandardCode);
            Assert.AreEqual(requiredPayment.JobId, earning.JobId);
            Assert.AreEqual(requiredPayment.IlrSubmissionDateTime, earning.IlrSubmissionDateTime);
        }

        private static IEnumerable<IFunctionalSkillEarningEvent> GetFunctionalSkillEarningEvents()
        {
            yield return CreateAct2FunctionalSkillEarningsEvent();
            yield return CreatePayableFunctionalSkillEarningsEvent();
        }
        private static Act2FunctionalSkillEarningsEvent CreateAct2FunctionalSkillEarningsEvent()
        {
            return new Act2FunctionalSkillEarningsEvent
            {
                CollectionYear = 1819,
                Learner = new Learner { ReferenceNumber = "R", Uln = uln },
                Ukprn = ukprn,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 7),
                LearningAim = new LearningAim
                {
                    FundingLineType = "flt",
                    PathwayCode = 3,
                    StandardCode = 4,
                    ProgrammeType = 5,
                    FrameworkCode = 6,
                    Reference = "7",
                    SequenceNumber = 1
                },
                JobId = 8,
                IlrSubmissionDateTime = DateTime.Today,
                StartDate = DateTime.Today.AddDays(-10),
                Earnings = new ReadOnlyCollection<FunctionalSkillEarning>(new List<FunctionalSkillEarning>
                {
                    new FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.OnProgrammeMathsAndEnglish, Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod {Period = 1, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 2, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 3, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 4, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 5, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 6, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 7, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 8, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 9, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 10, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 11, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 12, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                        })
                    }
                })
            };
        }

        private static PayableFunctionalSkillEarningEvent CreatePayableFunctionalSkillEarningsEvent()
        {
            return new PayableFunctionalSkillEarningEvent
            {
                CollectionYear = 1819,
                Learner = new Learner { ReferenceNumber = "R", Uln = uln },
                Ukprn = ukprn,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 7),
                LearningAim = new LearningAim
                {
                    FundingLineType = "flt",
                    PathwayCode = 3,
                    StandardCode = 4,
                    ProgrammeType = 5,
                    FrameworkCode = 6,
                    Reference = "7"
                },
                JobId = 8,
                IlrSubmissionDateTime = DateTime.Today,
                StartDate = DateTime.Today.AddDays(-10),
                Earnings = new ReadOnlyCollection<FunctionalSkillEarning>(new List<FunctionalSkillEarning>
                {
                    new FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.OnProgrammeMathsAndEnglish, Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod {Period = 1, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 2, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 3, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 4, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 5, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 6, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 7, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 8, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 9, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 10, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 11, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                            new EarningPeriod {Period = 12, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = 1},
                        })
                    }
                })
            };
        }

        private static PayableEarningEvent CreatePayableEarning()
        {
            return new PayableEarningEvent
            {
                EventId = Guid.NewGuid(),
                EarningEventId = Guid.NewGuid(),
                AgreementId = "103",
                CollectionYear = 1819,
                Learner = new Learner { ReferenceNumber = "R", Uln = 10 },
                Ukprn = 20,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 7),
                LearningAim = new LearningAim
                {
                    FundingLineType = "flt",
                    PathwayCode = 3,
                    StandardCode = 4,
                    ProgrammeType = 5,
                    FrameworkCode = 6,
                    Reference = "7"
                },
                JobId = 8,
                IlrSubmissionDateTime = DateTime.Today,
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning, Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod {Period = 1, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 2, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 3, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 4, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 5, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 6, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 7, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 8, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 9, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 10, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 11, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                            new EarningPeriod {Period = 12, Amount = 100, PriceEpisodeIdentifier = "1", SfaContributionPercentage = .9m, ApprenticeshipId = 102, AccountId = 101, ApprenticeshipPriceEpisodeId = 105, Priority = 104, AgreedOnDate = DateTime.Today},
                        })
                    }
                }
            };
        }
    }
}
