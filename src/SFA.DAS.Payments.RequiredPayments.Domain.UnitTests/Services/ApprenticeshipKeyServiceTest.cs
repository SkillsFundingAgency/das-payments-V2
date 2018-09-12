using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Enums;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests
{
    [TestFixture]
    public class ApprenticeshipKeyServiceTest
    {
        [Test]
        public void ApprenticeshipKeyContainsAllElements()
        {
            // arrange
            var learnerReferenceNumber = "1";
            var ukprn = 2;
            var frameworkCode = 3;
            var pathwayCode = 4;
            var programmeType = ProgrammeType.None;
            var standardCode = 5;
            var learnAimRef = "6";

            // act
            var key = new ApprenticeshipKeyService().GenerateApprenticeshipKey(ukprn, learnerReferenceNumber, frameworkCode, pathwayCode, programmeType, standardCode, learnAimRef);

            // assert
            Assert.AreEqual(0, key.IndexOf("2", StringComparison.Ordinal), "UKPRN should go first");
            Assert.Less(key.IndexOf("2", StringComparison.Ordinal), key.IndexOf("1", StringComparison.Ordinal), "LearnRefNumber should be after UKPRN");
            Assert.Less(key.IndexOf("1", StringComparison.Ordinal), key.IndexOf("3", StringComparison.Ordinal), "FrameworkCode should be after LearnRefNumber");
            Assert.Less(key.IndexOf("3", StringComparison.Ordinal), key.IndexOf("4", StringComparison.Ordinal), "PathwayCode should be after FrameworkCode");
            Assert.Less(key.IndexOf("4", StringComparison.Ordinal), key.IndexOf("0", StringComparison.Ordinal), "ProgrammeType should be after PathwayCode");
            Assert.Less(key.IndexOf("0", StringComparison.Ordinal), key.IndexOf("5", StringComparison.Ordinal), "StandardCode should be after ProgrammeType");
            Assert.Less(key.IndexOf("5", StringComparison.Ordinal), key.IndexOf("6", StringComparison.Ordinal), "LearnAimRef should be after StandardCode");
        }

        [Test]
        public void PaymentKeyContainsAllElements()
        {
            // arrange
            var priceEpisodeIdentifier = "1";
            var learnAimRef = "6";
            var transactionType = 3;
            var deliveryPeriod = new NamedCalendarPeriod {Name = "5"};

            // act
            var key = new ApprenticeshipKeyService().GeneratePaymentKey(priceEpisodeIdentifier, learnAimRef, transactionType, deliveryPeriod);

            // assert
            Assert.AreEqual(0, key.IndexOf("1", StringComparison.Ordinal), "PriceEpisodeIdentifier should go first");
            Assert.Less(key.IndexOf("1", StringComparison.Ordinal), key.IndexOf("6", StringComparison.Ordinal), "LearnAimRef should be after PriceEpisodeIdentifier");
            Assert.Less(key.IndexOf("6", StringComparison.Ordinal), key.IndexOf("3", StringComparison.Ordinal), "TransactionType should be after LearnAimRef");
            Assert.Less(key.IndexOf("3", StringComparison.Ordinal), key.IndexOf("5", StringComparison.Ordinal), "DeliveryPeriod should be after TransactionType");
        }
    }
}
