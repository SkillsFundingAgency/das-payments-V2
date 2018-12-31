﻿using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipContractType2EarningProcessorTest : ProcessorTestBase
    {
        private IApprenticeshipContractType2EarningProcessor earningProcessor;
        private long jobId;
        private long ukprn;

        [SetUp]
        public void SetUp()
        {
            earningProcessor = new ApprenticeshipContractType2EarningProcessor();
            jobId = 12345;
            ukprn = 12;
        }

        [Test]
        public void TestHandleOnProgrammeEarning()
        {
            // arrange
            var collectionPeriod = new CalendarPeriod("1819-R03");
            var earning = GetEarning();
            var onProgrammeEarning = new OnProgrammeEarning
            {
                Type = OnProgrammeEarningType.Learning,
                Periods = new ReadOnlyCollection<EarningPeriod>(new[]
                {
                    new EarningPeriod
                    {
                        Amount = 10,
                        Period = 01, // past
                        PriceEpisodeIdentifier = "13"
                    },
                    new EarningPeriod
                    {
                        Amount = 10,
                        Period = 02, // past
                        PriceEpisodeIdentifier = "13"
                    },
                    new EarningPeriod
                    {
                        Amount = 10,
                        Period = 03, // current
                        PriceEpisodeIdentifier = "13"
                    },
                    new EarningPeriod
                    {
                        Amount = 10,
                        Period = 04, // future
                        PriceEpisodeIdentifier = "13"
                    },
                    new EarningPeriod
                    {
                        Amount = 10,
                        Period = 05, // future
                        PriceEpisodeIdentifier = "13"
                    }
                })
            };
            earning.OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new[] { onProgrammeEarning });

            // act
            var paymentsDue = earningProcessor.HandleOnProgrammeEarning(Ukprn, JobId, onProgrammeEarning, collectionPeriod, earning.Learner, earning.LearningAim, earning.SfaContributionPercentage, earning.IlrSubmissionDateTime);

            // assert
            Assert.IsNotNull(paymentsDue);
            Assert.AreEqual(3, paymentsDue.Length);

            AssertPeriodsAreSame(earning, paymentsDue[0], onProgrammeEarning.Periods[0], collectionPeriod);
            AssertPeriodsAreSame(earning, paymentsDue[1], onProgrammeEarning.Periods[1], collectionPeriod);
            AssertPeriodsAreSame(earning, paymentsDue[2], onProgrammeEarning.Periods[2], collectionPeriod);
        }

       

        [Test]
        public void TestHandleZeroAmountEarning()
        {
            // arrange
            var collectionPeriod = new CalendarPeriod("1819-R03");

            var earning = GetEarning();
            var onProgrammeEarning = new OnProgrammeEarning
            {
                Type = OnProgrammeEarningType.Learning,
                Periods = new ReadOnlyCollection<EarningPeriod>(new[]
                {
                    new EarningPeriod
                    {
                        Amount = 0,
                        Period = 1,
                        PriceEpisodeIdentifier = "13"
                    }
                })
            };

            // act
            var paymentsDue = earningProcessor.HandleOnProgrammeEarning(Ukprn, JobId, onProgrammeEarning, collectionPeriod, earning.Learner, earning.LearningAim, earning.SfaContributionPercentage, earning.IlrSubmissionDateTime);

            // assert
            Assert.IsNotNull(paymentsDue);
            Assert.AreEqual(1, paymentsDue.Length);
        }

        private ApprenticeshipContractType2EarningEvent GetEarning()
        {
            var earning = new ApprenticeshipContractType2EarningEvent
            {
                EventTime = DateTimeOffset.UtcNow,
                Learner = new Learner
                {
                    ReferenceNumber = "1",
                    Uln = 3
                },
                Ukprn = ukprn,
                JobId = jobId,
                LearningAim = new LearningAim
                {
                    FrameworkCode = 5,
                    FundingLineType = "6",
                    PathwayCode = 7,
                    ProgrammeType = 8,
                    Reference = "9",
                    StandardCode = 10
                },
                PriceEpisodes = new ReadOnlyCollection<PriceEpisode>(new[]
                {
                    new PriceEpisode
                    {
                        TotalNegotiatedPrice1 = 120,
                        StartDate = new DateTime(2018, 8, 1),
                        PlannedEndDate = new DateTime(2019, 7, 31),
                        Identifier = "13"
                    }
                }),
                IncentiveEarnings = new ReadOnlyCollection<IncentiveEarning>(new IncentiveEarning[0]),
                SfaContributionPercentage = 100
            };
            return earning;
        }

        [Test]
        public void TestNullEarning()
        {
            try
            {
                earningProcessor.HandleOnProgrammeEarning(Ukprn, JobId, null, new CalendarPeriod("1718-R02"), new Learner(), new LearningAim(), 100, DateTime.UtcNow);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("onProgEarning", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestNullCollectionPeriod()
        {
            try
            {
                earningProcessor.HandleOnProgrammeEarning(Ukprn, JobId, new OnProgrammeEarning(), null, new Learner(), new LearningAim(), 100, DateTime.UtcNow);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("collectionPeriod", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestNullLearnerPeriod()
        {
            try
            {
                earningProcessor.HandleOnProgrammeEarning(Ukprn, JobId, new OnProgrammeEarning(), new CalendarPeriod("1718-R02"), null, new LearningAim(), 100, DateTime.UtcNow);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("learner", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestNullLearnAimPeriod()
        {
            try
            {
                earningProcessor.HandleOnProgrammeEarning(Ukprn, JobId, new OnProgrammeEarning(), new CalendarPeriod("1718-R02"), new Learner(), null, 100, DateTime.UtcNow);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("learningAim", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        private static void AssertPeriodsAreSame(ApprenticeshipContractTypeEarningsEvent earning,
                                                ApprenticeshipContractTypePaymentDueEvent paymentDue, 
                                                EarningPeriod earningPeriod, 
                                                CalendarPeriod collectionPeriod)
        {
            Assert.AreEqual(earning.Ukprn, paymentDue.Ukprn);
            Assert.AreEqual(earning.SfaContributionPercentage, paymentDue.SfaContributionPercentage);
            Assert.AreEqual(earning.PriceEpisodes[0].Identifier, paymentDue.PriceEpisodeIdentifier);
            Assert.AreEqual(earning.OnProgrammeEarnings[0].Type, paymentDue.Type);
            Assert.AreEqual(earningPeriod.Period, paymentDue.DeliveryPeriod.Period);
            Assert.AreEqual(earningPeriod.Amount, paymentDue.AmountDue);
            Assert.AreEqual(earning.Learner.ReferenceNumber, paymentDue.Learner.ReferenceNumber);
            Assert.AreEqual(earning.Learner.Uln, paymentDue.Learner.Uln);
            Assert.AreEqual(earning.LearningAim.FrameworkCode, paymentDue.LearningAim.FrameworkCode);
            Assert.AreEqual(earning.LearningAim.FundingLineType, paymentDue.LearningAim.FundingLineType);
            Assert.AreEqual(earning.LearningAim.PathwayCode, paymentDue.LearningAim.PathwayCode);
            Assert.AreEqual(earning.LearningAim.ProgrammeType, paymentDue.LearningAim.ProgrammeType);
            Assert.AreEqual(earning.LearningAim.Reference, paymentDue.LearningAim.Reference);
            Assert.AreEqual(earning.LearningAim.StandardCode, paymentDue.LearningAim.StandardCode);
            Assert.AreEqual(collectionPeriod, paymentDue.CollectionPeriod);
        }

       
    }
}
