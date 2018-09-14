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
    public class Act2PaymentDueProcessorTest
    {
        private Act2PaymentDueProcessor _act2PaymentDueProcessor;

        [Test]
        public void TestNullEarning()
        {
            // arrange
            _act2PaymentDueProcessor = new Act2PaymentDueProcessor();

            // act
            // assert
            try
            {
                _act2PaymentDueProcessor.ProcessPaymentDue(null, null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("paymentDue", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestNullPaymentHistory()
        {
            // arrange
            _act2PaymentDueProcessor = new Act2PaymentDueProcessor();

            // act
            // assert
            try
            {
                _act2PaymentDueProcessor.ProcessPaymentDue(new ApprenticeshipContractType2PaymentDueEvent(), null);
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
            _act2PaymentDueProcessor = new Act2PaymentDueProcessor();

            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                AmountDue = 1,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 1, Name = "1819R01"},
                CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 1, Name = "1819R01"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

            var history = new Payment[0];

            // act
            var requiredPayment = _act2PaymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNotNull(requiredPayment);
            Assert.AreEqual(1, requiredPayment.AmountDue);
            Assert.AreEqual(paymentDue.LearningAim.Reference, requiredPayment.LearningAim.Reference);
        }

        [Test]
        public void TestMultiplePaymentsFoundWithLessPaidThanDue()
        {
            // arrange
            _act2PaymentDueProcessor = new Act2PaymentDueProcessor();

            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                AmountDue = 5,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

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
            var requiredPayment = _act2PaymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNotNull(requiredPayment);
            Assert.AreEqual(1, requiredPayment.AmountDue);
        }

        [Test]
        public void TestOnePaymentFound()
        {
            // arrange
            _act2PaymentDueProcessor = new Act2PaymentDueProcessor();

            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                AmountDue = 2,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

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
            var requiredPayment = _act2PaymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNull(requiredPayment);
        }
        
        [Test]
        public void TestMultiplePaymentsFoundWithMorePaidThanDue()
        {
            // arrange
            _act2PaymentDueProcessor = new Act2PaymentDueProcessor();

            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                AmountDue = 5,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

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
            var requiredPayment = _act2PaymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNotNull(requiredPayment);
            Assert.AreEqual(-1, requiredPayment.AmountDue);
        }
              
        [Test]
        public void TestMultiplePaymentsFoundWithSamePaidAsDue()
        {
            // arrange
            _act2PaymentDueProcessor = new Act2PaymentDueProcessor();

            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                AmountDue = 5,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new CalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

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
            var requiredPayment = _act2PaymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNull(requiredPayment);
        }
    }
}
