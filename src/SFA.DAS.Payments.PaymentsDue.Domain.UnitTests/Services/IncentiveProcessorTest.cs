using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Domain.UnitTests.Services
{
    public class IncentiveProcessorTest : ProcessorTestBase
    {
        private IIncentiveProcessor incentiveProcessor;

        [SetUp]
        public void SetUp()
        {
            incentiveProcessor = new IncentiveProcessor();
        }

        [Test]
        [TestCaseSource(nameof(GetIncentiveTypes))]
        public void TestHandleIncentiveEarning(IncentiveEarningType type)
        {
            // arrange
            var collectionPeriod = new CalendarPeriod("1819-R03");
            var earning = GetEarning();

            var incentiveEarning = new IncentiveEarning
            {
                Type = type,
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
            earning.IncentiveEarnings = new ReadOnlyCollection<IncentiveEarning>(new[] { incentiveEarning });

            // act
            var paymentsDue = incentiveProcessor.HandleIncentiveEarnings(Ukprn, JobId, incentiveEarning, collectionPeriod, earning.Learner, earning.LearningAim, earning.SfaContributionPercentage, earning.IlrSubmissionDateTime);

            // assert
            Assert.IsNotNull(paymentsDue);
            Assert.AreEqual(3, paymentsDue.Length);

            AssertPeriodsAreSame(earning, paymentsDue[0], incentiveEarning.Periods[0], collectionPeriod);
            AssertPeriodsAreSame(earning, paymentsDue[1], incentiveEarning.Periods[1], collectionPeriod);
            AssertPeriodsAreSame(earning, paymentsDue[2], incentiveEarning.Periods[2], collectionPeriod);
        }

        private static void AssertPeriodsAreSame(ApprenticeshipContractTypeEarningsEvent earning,
            IncentivePaymentDueEvent paymentDue,
            EarningPeriod earningPeriod,
            CalendarPeriod collectionPeriod)
        {
            Assert.AreEqual(earning.Ukprn, paymentDue.Ukprn);
            Assert.AreEqual(earning.PriceEpisodes[0].Identifier, paymentDue.PriceEpisodeIdentifier);
            Assert.AreEqual((int)earning.IncentiveEarnings[0].Type, (int)paymentDue.Type);
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

        private static IEnumerable<IncentiveEarningType> GetIncentiveTypes()
        {
            return Enum.GetValues(typeof(IncentiveEarningType)).Cast<IncentiveEarningType>();
        }
    }
}