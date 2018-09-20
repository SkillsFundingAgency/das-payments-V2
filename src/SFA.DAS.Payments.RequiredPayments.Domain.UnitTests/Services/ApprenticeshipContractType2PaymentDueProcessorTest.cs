using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipContractType2PaymentDueProcessorTest
    {
        private ApprenticeshipContractType2PaymentDueProcessor _act2PaymentDueProcessor;

        [Test]
        public void TestNullPaymentHistory()
        {
            // arrange
            _act2PaymentDueProcessor = new ApprenticeshipContractType2PaymentDueProcessor();

            // act
            // assert
            try
            {
                _act2PaymentDueProcessor.CalculateRequiredPaymentAmount(0, null);
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
            _act2PaymentDueProcessor = new ApprenticeshipContractType2PaymentDueProcessor();

            var history = new Payment[0];

            // act
            var amount = _act2PaymentDueProcessor.CalculateRequiredPaymentAmount(1, history);

            // assert
            Assert.IsNotNull(amount);
            Assert.AreEqual(1, amount);
        }

        [Test]
        public void TestMultiplePaymentsFoundWithLessPaidThanDue()
        {
            // arrange
            _act2PaymentDueProcessor = new ApprenticeshipContractType2PaymentDueProcessor();

            var history = new []
            {
                new Payment
                {
                    Amount = 1,
                    DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var amount = _act2PaymentDueProcessor.CalculateRequiredPaymentAmount(5, history);

            // assert
            Assert.AreEqual(1, amount);
        }

        [Test]
        public void TestOnePaymentFound()
        {
            // arrange
            _act2PaymentDueProcessor = new ApprenticeshipContractType2PaymentDueProcessor();

            var history = new []
            {
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = new CalendarPeriod{ Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new CalendarPeriod{ Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var amount = _act2PaymentDueProcessor.CalculateRequiredPaymentAmount(2, history);

            // assert
            Assert.AreEqual(0, amount);
        }
        
        [Test]
        public void TestMultiplePaymentsFoundWithMorePaidThanDue()
        {
            // arrange
            _act2PaymentDueProcessor = new ApprenticeshipContractType2PaymentDueProcessor();

            var history = new []
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var amount = _act2PaymentDueProcessor.CalculateRequiredPaymentAmount(5, history);

            // assert
            Assert.AreEqual(-1, amount);
        }
              
        [Test]
        public void TestMultiplePaymentsFoundWithSamePaidAsDue()
        {
            // arrange
            _act2PaymentDueProcessor = new ApprenticeshipContractType2PaymentDueProcessor();

            var history = new []
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                },
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var amount = _act2PaymentDueProcessor.CalculateRequiredPaymentAmount(5, history);

            // assert
            Assert.AreEqual(0, amount);
        }
    }
}
