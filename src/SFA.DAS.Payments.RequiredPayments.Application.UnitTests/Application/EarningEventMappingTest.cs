using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application
{
    [TestFixture]
    public class EarningEventMappingTest
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [Test]
        public void TestPayableEarningEventMap()
        {
            // arrange
            var payableEarning = CreatePayableEarning();
            RequiredPaymentEvent requiredPayment = new ApprenticeshipContractType1RequiredPaymentEvent();

            // act
            mapper.Map(payableEarning, requiredPayment);

            // assert
            AssertCommonProperties(requiredPayment, payableEarning);

            var act1RequiredPayment = (ApprenticeshipContractType1RequiredPaymentEvent)requiredPayment;

            Assert.AreEqual(payableEarning.EmployerAccountId, act1RequiredPayment.EmployerAccountId);
            Assert.AreEqual(payableEarning.CommitmentId, act1RequiredPayment.CommitmentId);
            Assert.AreEqual(payableEarning.AgreementId, act1RequiredPayment.AgreementId);

        }

        [Test]
        public void TestFunctionalSkillEarningEventMap()
        {
            // arrange
            var functionalSkillEarningsEvent = CreateFunctionalSkillEarningsEvent();
            RequiredPaymentEvent requiredPayment = new IncentiveRequiredPaymentEvent();

            // act
            mapper.Map(functionalSkillEarningsEvent, requiredPayment);

            // assert
            AssertCommonProperties(requiredPayment, functionalSkillEarningsEvent);
        }

        private static void AssertCommonProperties(RequiredPaymentEvent requiredPayment, IEarningEvent earning)
        {
            Assert.AreNotSame(requiredPayment.Learner, earning.Learner);
            Assert.AreEqual(requiredPayment.Learner.Uln, earning.Learner.Uln);
            Assert.AreEqual(requiredPayment.Learner.ReferenceNumber, earning.Learner.ReferenceNumber);
            Assert.AreEqual(requiredPayment.Ukprn, earning.Ukprn);
            Assert.AreNotSame(requiredPayment.CollectionPeriod, earning.CollectionPeriod);
            Assert.AreEqual(requiredPayment.CollectionPeriod.Name, earning.CollectionPeriod.Name);
            Assert.AreNotSame(requiredPayment.LearningAim, earning.LearningAim);
            Assert.AreEqual(requiredPayment.LearningAim.PathwayCode, earning.LearningAim.PathwayCode);
            Assert.AreEqual(requiredPayment.LearningAim.FrameworkCode, earning.LearningAim.FrameworkCode);
            Assert.AreEqual(requiredPayment.LearningAim.FundingLineType, earning.LearningAim.FundingLineType);
            Assert.AreEqual(requiredPayment.LearningAim.ProgrammeType, earning.LearningAim.ProgrammeType);
            Assert.AreEqual(requiredPayment.LearningAim.Reference, earning.LearningAim.Reference);
            Assert.AreEqual(requiredPayment.LearningAim.StandardCode, earning.LearningAim.StandardCode);
            Assert.AreEqual(requiredPayment.JobId, earning.JobId);
            Assert.AreEqual(requiredPayment.IlrSubmissionDateTime, earning.IlrSubmissionDateTime);
        }

        private static FunctionalSkillEarningsEvent CreateFunctionalSkillEarningsEvent()
        {
            return new FunctionalSkillEarningsEvent
            {
                CollectionYear = 1819,
                Learner = new Learner {ReferenceNumber = "R", Uln = 10},
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
                EmployerAccountId = 101,
                CommitmentId = 102,
                AgreementId = "103",
                CollectionYear = 1819,
                Learner = new Learner {ReferenceNumber = "R", Uln = 10},
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
                OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning, Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
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
    }
}
