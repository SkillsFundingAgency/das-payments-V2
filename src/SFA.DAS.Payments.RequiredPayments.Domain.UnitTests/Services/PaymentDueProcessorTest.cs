using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Factories;
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
            var history = new List<Payment>();

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

            var history = new List<Payment>
            {
                new Payment
                {
                    Amount = 1,
                    DeliveryPeriod = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
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
            var history = new List<Payment>
            {
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
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
            var history = new List<Payment>
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
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
            var history = new List<Payment>
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                },
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                }
            };

            // act
            var amount = paymentDueProcessor.CalculateRequiredPaymentAmount(5, history);

            // assert
            Assert.AreEqual(0, amount);
        }
    }
}
