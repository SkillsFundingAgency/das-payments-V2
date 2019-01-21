using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class PaymentDueProcessorTest
    {
        private PaymentDueProcessor paymentDueProcessor;

        [SetUp]
        public void SetUp()
        {
            paymentDueProcessor = new PaymentDueProcessor();
        }

        [Test]
        public void TestNullPaymentHistory()
        {
            // arrange
            // act
            // assert
            try
            {
                paymentDueProcessor.CalculateRequiredPaymentAmount(0, null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("paymentHistory", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestNoPaymentFound()
        {
            // arrange
            var history = new Payment[0];

            // act
            var amount = paymentDueProcessor.CalculateRequiredPaymentAmount(1, history);

            // assert
            Assert.IsNotNull(amount);
            Assert.AreEqual(1, amount);
        }

        [Test]
        public void TestMultiplePaymentsFoundWithLessPaidThanDue()
        {
            // arrange
            paymentDueProcessor = new PaymentDueProcessor();

            var history = new []
            {
                new Payment
                {
                    Amount = 1,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02"),
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02"),
                }
            };

            // act
            var amount = paymentDueProcessor.CalculateRequiredPaymentAmount(5, history);

            // assert
            Assert.AreEqual(1, amount);
        }

        [Test]
        public void TestOnePaymentFound()
        {
            // arrange
            var history = new []
            {
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02")
                }
            };

            // act
            var amount = paymentDueProcessor.CalculateRequiredPaymentAmount(2, history);

            // assert
            Assert.AreEqual(0, amount);
        }
        
        [Test]
        public void TestMultiplePaymentsFoundWithMorePaidThanDue()
        {
            // arrange
            var history = new []
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02"),
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02"),
                }
            };

            // act
            var amount = paymentDueProcessor.CalculateRequiredPaymentAmount(5, history);

            // assert
            Assert.AreEqual(-1, amount);
        }
              
        [Test]
        public void TestMultiplePaymentsFoundWithSamePaidAsDue()
        {
            // arrange
            var history = new []
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02"),
                },
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02")
                }
            };

            // act
            var amount = paymentDueProcessor.CalculateRequiredPaymentAmount(5, history);

            // assert
            Assert.AreEqual(0, amount);
        }

        [Test]
        [TestCase(.89, 100, 0, .89)]
        [TestCase(.89, 0, 1, .89)]
        [TestCase(.89, 0, 0, .89)]
        [TestCase(0, 100, 0, 0)]
        [TestCase(0, 100, 2, 0)]
        [TestCase(0, 0, 2, .5)]
        [TestCase(0, 0, 1, 1)]
        public void TestCalculateSfaContributionPercentageDefaultsToEarning(decimal earningPercentage, decimal amountDue, int numberOfHistoricalPayments, decimal expectedResult)
        {
            // arrange
            var history = new Payment[numberOfHistoricalPayments];
            for (int i = 0; i < numberOfHistoricalPayments; i++)
            {
                history[i] = new Payment
                {
                    Amount = 100,
                    DeliveryPeriod = new CalendarPeriod("1819R02"),
                    CollectionPeriod = new CalendarPeriod("1819R02"),
                    PriceEpisodeIdentifier = "1",
                    FundingSource = i % 2 == 0 ? FundingSourceType.CoInvestedSfa : FundingSourceType.CoInvestedEmployer
                };
            }

            // act
            var actualResult = paymentDueProcessor.CalculateSfaContributionPercentage(earningPercentage, amountDue, history);

            // assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
