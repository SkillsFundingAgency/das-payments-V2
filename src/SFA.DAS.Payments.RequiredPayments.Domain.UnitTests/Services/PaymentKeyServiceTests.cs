using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class PaymentKeyServiceTests
    {
        [Test]
        public void PaymentKeyContainsAllElements()
        {
            // arrange
            var priceEpisodeIdentifier = "1";
            var learnAimRef = "6";
            var transactionType = 3;
            var deliveryPeriod = new CalendarPeriod("1819", 5);

            // act
            var key = new PaymentKeyService().GeneratePaymentKey(priceEpisodeIdentifier, learnAimRef, transactionType, deliveryPeriod);

            // assert
            Assert.AreEqual(0, key.IndexOf("1", StringComparison.Ordinal), "PriceEpisodeIdentifier should go first");
            Assert.Less(key.IndexOf("1", StringComparison.Ordinal), key.IndexOf("6", StringComparison.Ordinal), "LearnAimRef should be after PriceEpisodeIdentifier");
            Assert.Less(key.IndexOf("6", StringComparison.Ordinal), key.IndexOf("3", StringComparison.Ordinal), "TransactionType should be after LearnAimRef");
            Assert.Less(key.IndexOf("3", StringComparison.Ordinal), key.IndexOf("5", StringComparison.Ordinal), "DeliveryPeriod should be after TransactionType");
        }

        [Test]
        public void TestPaymentKeyChangesCase()
        {
            // arrange
            var priceEpisodeIdentifier = "A";
            var learnAimRef = "B";
            var transactionType = 3;
            var deliveryPeriod = new CalendarPeriod("1819", 5);

            // act
            var key = new PaymentKeyService().GeneratePaymentKey(priceEpisodeIdentifier, learnAimRef, transactionType, deliveryPeriod);

            // assert
            Assert.IsFalse(key.Contains("A"));
            Assert.IsFalse(key.Contains("B"));
            Assert.IsTrue(key.Contains("a"));
            Assert.IsTrue(key.Contains("b"));
        }


    }
}