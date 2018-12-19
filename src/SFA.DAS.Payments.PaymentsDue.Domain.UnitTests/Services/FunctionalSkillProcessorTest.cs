using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain.UnitTests.Services
{
    public class FunctionalSkillProcessorTest : ProcessorTestBase
    {
        private IFunctionalSkillsEarningProcessor processor;

        [SetUp]
        public void SetUp()
        {
            processor = new FunctionalSkillsEarningProcessor();
        }

        [Test]
        public void TestHandleIncentiveEarning()
        {
            // arrange
            var collectionPeriod = new CalendarPeriod("1819-R03");
            var learner = new Learner
            {
                ReferenceNumber = "1",
                Uln = 3
            };
            var learningAim = new LearningAim
            {
                FrameworkCode = 5,
                FundingLineType = "6",
                PathwayCode = 7,
                ProgrammeType = 8,
                Reference = "9",
                StandardCode = 10
            };

            var functionalSkillEarning = new FunctionalSkillEarning
            {
                Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
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

            var submission = new Submission { Ukprn = Ukprn, JobId = JobId, CollectionPeriod = collectionPeriod, IlrSubmissionDate = DateTime.Now };

            // act
            var paymentsDue = processor.HandleEarning(submission, functionalSkillEarning, learner, learningAim, ContractType.Act2);

            // assert
            Assert.IsNotNull(paymentsDue);
            Assert.AreEqual(3, paymentsDue.Length);

            AssertPeriodsAreSame(functionalSkillEarning, paymentsDue[0], functionalSkillEarning.Periods[0], learner, learningAim, submission);
            AssertPeriodsAreSame(functionalSkillEarning, paymentsDue[1], functionalSkillEarning.Periods[1], learner, learningAim, submission);
            AssertPeriodsAreSame(functionalSkillEarning, paymentsDue[2], functionalSkillEarning.Periods[2], learner, learningAim, submission);            
        }

        private static void AssertPeriodsAreSame(FunctionalSkillEarning earning,
            IncentivePaymentDueEvent paymentDue,
            EarningPeriod earningPeriod,
            Learner learner,
            LearningAim learningAim,
            Submission submission)
        {
            Assert.AreEqual(submission.Ukprn, paymentDue.Ukprn);
            Assert.AreEqual((int)earning.Type, (int)paymentDue.Type);
            Assert.AreEqual(earningPeriod.Period, paymentDue.DeliveryPeriod.Period);
            Assert.AreEqual(earningPeriod.Amount, paymentDue.AmountDue);
            Assert.AreEqual(learner.ReferenceNumber, paymentDue.Learner.ReferenceNumber);
            Assert.AreEqual(learner.Uln, paymentDue.Learner.Uln);
            Assert.AreEqual(learningAim.FrameworkCode, paymentDue.LearningAim.FrameworkCode);
            Assert.AreEqual(learningAim.FundingLineType, paymentDue.LearningAim.FundingLineType);
            Assert.AreEqual(learningAim.PathwayCode, paymentDue.LearningAim.PathwayCode);
            Assert.AreEqual(learningAim.ProgrammeType, paymentDue.LearningAim.ProgrammeType);
            Assert.AreEqual(learningAim.Reference, paymentDue.LearningAim.Reference);
            Assert.AreEqual(learningAim.StandardCode, paymentDue.LearningAim.StandardCode);
            Assert.AreEqual(submission.CollectionPeriod, paymentDue.CollectionPeriod);
        }
    }
}