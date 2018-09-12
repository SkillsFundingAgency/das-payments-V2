using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class EarningProcessorTest
    {
        private PaymentDueProcessor _paymentDueProcessor;

        [Test]
        public void TestNullEarning()
        {
            // arrange
            _paymentDueProcessor = new PaymentDueProcessor();

            // act
            // assert
            try
            {
                _paymentDueProcessor.ProcessPaymentDue(null, null);
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
            _paymentDueProcessor = new PaymentDueProcessor();

            // act
            // assert
            try
            {
                _paymentDueProcessor.ProcessPaymentDue(new PaymentDueEvent(), null);
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
            _paymentDueProcessor = new PaymentDueProcessor();

            var paymentDue = new OnProgrammePaymentDueEvent
            {
                Amount = 1,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 1, Name = "1819R01"},
                CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 1, Name = "1819R01"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

            var history = new Payment[0];

            // act
            var requiredPayment = _paymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNotNull(requiredPayment);
            Assert.AreEqual(1, requiredPayment.Amount);
            Assert.AreSame(paymentDue.LearningAim, requiredPayment.LearningAim);
        }

        [Test]
        public void TestMultiplePaymentsFoundWithLessPaidThanDue()
        {
            // arrange
            _paymentDueProcessor = new PaymentDueProcessor();

            var paymentDue = new OnProgrammePaymentDueEvent
            {
                Amount = 5,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

            var history = new []
            {
                new Payment
                {
                    Amount = 1,
                    DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var requiredPayment = _paymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNotNull(requiredPayment);
            Assert.AreEqual(1, requiredPayment.Amount);
        }

        [Test]
        public void TestOnePaymentFound()
        {
            // arrange
            _paymentDueProcessor = new PaymentDueProcessor();

            var paymentDue = new OnProgrammePaymentDueEvent
            {
                Amount = 2,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

            var history = new []
            {
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = new NamedCalendarPeriod{ Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new NamedCalendarPeriod{ Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var requiredPayment = _paymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNull(requiredPayment);
        }
        
        [Test]
        public void TestMultiplePaymentsFoundWithMorePaidThanDue()
        {
            // arrange
            _paymentDueProcessor = new PaymentDueProcessor();

            var paymentDue = new OnProgrammePaymentDueEvent
            {
                Amount = 5,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

            var history = new []
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                },
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var requiredPayment = _paymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNotNull(requiredPayment);
            Assert.AreEqual(-1, requiredPayment.Amount);
        }
              
        [Test]
        public void TestMultiplePaymentsFoundWithSamePaidAsDue()
        {
            // arrange
            _paymentDueProcessor = new PaymentDueProcessor();

            var paymentDue = new OnProgrammePaymentDueEvent
            {
                Amount = 5,
                LearningAim = new LearningAim {Reference = "1"},
                Learner = new Learner(),
                DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

            var history = new []
            {
                new Payment
                {
                    Amount = 3,
                    DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                },
                new Payment
                {
                    Amount = 2,
                    DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                    CollectionPeriod = new NamedCalendarPeriod {Year = 2018, Month = 2, Name = "1819R02"},
                }
            };

            // act
            var requiredPayment = _paymentDueProcessor.ProcessPaymentDue(paymentDue, history);

            // assert
            Assert.IsNull(requiredPayment);
        }
    }
}
