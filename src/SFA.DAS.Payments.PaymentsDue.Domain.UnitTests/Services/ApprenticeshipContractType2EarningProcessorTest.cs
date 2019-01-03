using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipContractType2EarningProcessorTest : ProcessorTestBase
    {
        private IApprenticeshipContractType2EarningProcessor earningProcessor;

        [SetUp]
        public void SetUp()
        {
            earningProcessor = new ApprenticeshipContractType2EarningProcessor();
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

            var submission = new Submission {Ukprn = Ukprn, JobId = JobId, CollectionPeriod = collectionPeriod, IlrSubmissionDate = earning.IlrSubmissionDateTime};

            // act
            var paymentsDue = earningProcessor.HandleOnProgrammeEarning(submission, onProgrammeEarning, earning.Learner, earning.LearningAim, earning.SfaContributionPercentage);

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
            var submission = new Submission { Ukprn = Ukprn, JobId = JobId, CollectionPeriod = collectionPeriod, IlrSubmissionDate = earning.IlrSubmissionDateTime };

            // act
            var paymentsDue = earningProcessor.HandleOnProgrammeEarning(submission, onProgrammeEarning, earning.Learner, earning.LearningAim, earning.SfaContributionPercentage);

            // assert
            Assert.IsNotNull(paymentsDue);
            Assert.AreEqual(1, paymentsDue.Length);
        }

        [Test]
        public void TestNullEarning()
        {
            var submission = new Submission { Ukprn = Ukprn, JobId = JobId, CollectionPeriod = new CalendarPeriod("1718-R02"), IlrSubmissionDate = DateTime.Now };
            try
            {
                earningProcessor.HandleOnProgrammeEarning(submission, null, new Learner(), new LearningAim(), 100);
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
            var submission = new Submission { Ukprn = Ukprn, JobId = JobId, CollectionPeriod = null, IlrSubmissionDate = DateTime.Now };
            try
            {
                earningProcessor.HandleOnProgrammeEarning(submission, new OnProgrammeEarning(), new Learner(), new LearningAim(), 100);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual(nameof(submission.CollectionPeriod), ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestNullLearnerPeriod()
        {
            var submission = new Submission { Ukprn = Ukprn, JobId = JobId, CollectionPeriod = new CalendarPeriod("1718-R02"), IlrSubmissionDate = DateTime.Now };
            try
            {
                earningProcessor.HandleOnProgrammeEarning(submission, new OnProgrammeEarning(), null, new LearningAim(), 100);
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
            var submission = new Submission { Ukprn = Ukprn, JobId = JobId, CollectionPeriod = new CalendarPeriod("1718-R02"), IlrSubmissionDate = DateTime.Now };
            try
            {
                earningProcessor.HandleOnProgrammeEarning(submission, new OnProgrammeEarning(), new Learner(), null, 100);
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
