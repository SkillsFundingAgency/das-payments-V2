using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
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
        public void TestEarningEventMap()
        {
            // arrange
            PayableEarningEvent payableEarning = CreateEarning();

            // act
            var requiredPayments = mapper.Map<ReadOnlyCollection<RequiredPaymentEvent>>(payableEarning);

            // assert
            Assert.IsNotNull(requiredPayments);
            Assert.IsNotEmpty(requiredPayments);
            Assert.AreEqual(12, requiredPayments.Count);

            for (var i = 1; i < 13; i++)
            {
                var requiredPayment = requiredPayments[i - 1];
                Assert.AreEqual(100, requiredPayment.AmountDue);
                Assert.AreEqual("1", requiredPayment.PriceEpisodeIdentifier);
                Assert.AreEqual(i, requiredPayment.DeliveryPeriod.Period);

                Assert.IsInstanceOf<ApprenticeshipContractType1RequiredPaymentEvent>(requiredPayment);
                Assert.AreNotSame(requiredPayment.Learner, payableEarning.Learner);
                Assert.AreEqual(requiredPayment.Learner.Uln, payableEarning.Learner.Uln);
                Assert.AreEqual(requiredPayment.Learner.ReferenceNumber, payableEarning.Learner.ReferenceNumber);
                Assert.AreEqual(requiredPayment.Ukprn, payableEarning.Ukprn);
                Assert.AreEqual(requiredPayment.CollectionPeriod.Name, payableEarning.CollectionPeriod.Name);
                Assert.AreNotSame(requiredPayment.LearningAim, payableEarning.LearningAim);
                Assert.AreEqual(requiredPayment.LearningAim.PathwayCode, payableEarning.LearningAim.PathwayCode);
                Assert.AreEqual(requiredPayment.LearningAim.FrameworkCode, payableEarning.LearningAim.FrameworkCode);
                Assert.AreEqual(requiredPayment.LearningAim.FundingLineType, payableEarning.LearningAim.FundingLineType);
                Assert.AreEqual(requiredPayment.LearningAim.ProgrammeType, payableEarning.LearningAim.ProgrammeType);
                Assert.AreEqual(requiredPayment.LearningAim.Reference, payableEarning.LearningAim.Reference);
                Assert.AreEqual(requiredPayment.LearningAim.StandardCode, payableEarning.LearningAim.StandardCode);
                Assert.AreEqual(requiredPayment.JobId, payableEarning.JobId);
                Assert.AreEqual(requiredPayment.IlrSubmissionDateTime, payableEarning.IlrSubmissionDateTime);
            }
        }

        [Test]
        public void TestPayableEarningEventMap()
        {
            // arrange
            var payableEarning = (PayableEarningEvent)CreateEarning();

            // act
            var requiredPayments = mapper.Map<ReadOnlyCollection<RequiredPaymentEvent>>(payableEarning);

            // assert
            Assert.IsNotNull(requiredPayments);
            Assert.IsNotEmpty(requiredPayments);
            Assert.AreEqual(12, requiredPayments.Count);

            for (var i = 1; i < 13; i++)
            {
                var requiredPayment = requiredPayments[i - 1];
                var act1RequiredPayment = requiredPayment as ApprenticeshipContractType1RequiredPaymentEvent;
                Assert.IsNotNull(act1RequiredPayment);
                Assert.AreEqual(payableEarning.EmployerAccountId, act1RequiredPayment.EmployerAccountId);
                Assert.AreEqual(payableEarning.CommitmentId, act1RequiredPayment.CommitmentId);
                Assert.AreEqual(payableEarning.AgreementId, act1RequiredPayment.AgreementId);
            }
        }

        private static PayableEarningEvent CreateEarning()
        {
            return new PayableEarningEvent
            {
                EmployerAccountId = 101,
                CommitmentId = 102,
                AgreementId = "103",
                CollectionYear = "1819",
                Learner = new Learner {ReferenceNumber = "R", Uln = 10},
                Ukprn = 20,
                CollectionPeriod = new CalendarPeriod(2019, 1),
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
