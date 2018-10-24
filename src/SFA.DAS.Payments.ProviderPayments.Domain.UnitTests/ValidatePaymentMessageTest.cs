using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;

namespace SFA.DAS.Payments.ProviderPayments.Domain.UnitTests
{
    [TestFixture]
    public class ValidatePaymentMessageTest
    {
        private ValidatePaymentMessage validatePaymentMessage;

        [Test]
        public void ShouldBeValidIfCurrentIlrIsNull()
        {
            validatePaymentMessage = new ValidatePaymentMessage();

            var result = validatePaymentMessage.IsLatestIlrPayment(new PaymentMessageValidationRequest
            {
                CurrentIlr = null,
            });

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldBeValidIfCurrentIlrMatchesIncomingMessage()
        {
            var currentIlr = new IlrSubmittedEvent
            {
                Ukprn = 1000,
                JobId = 1000,
                IlrSubmissionDateTime = DateTime.MaxValue
            };

            validatePaymentMessage = new ValidatePaymentMessage();

            var result = validatePaymentMessage.IsLatestIlrPayment(new PaymentMessageValidationRequest
            {
                CurrentIlr = currentIlr,
                IncomingPaymentUkprn = currentIlr.Ukprn,
                IncomingPaymentJobId = currentIlr.JobId,
                IncomingPaymentSubmissionDate = DateTime.MaxValue
            });

            Assert.IsTrue(result);
        }


        [Test]
        public void ShouldBeInValidIfCurrentIlrDoNotMatchIncomingMessage()
        {
            var currentIlr = new IlrSubmittedEvent
            {
                Ukprn = 1000,
                JobId = 1000,
                IlrSubmissionDateTime  = DateTime.MaxValue
            };

            validatePaymentMessage = new ValidatePaymentMessage();

            var result = validatePaymentMessage.IsLatestIlrPayment(new PaymentMessageValidationRequest
            {
                CurrentIlr = currentIlr,
                IncomingPaymentUkprn = currentIlr.Ukprn,
                IncomingPaymentJobId = currentIlr.JobId,
                IncomingPaymentSubmissionDate = DateTime.MinValue
            });

            Assert.IsFalse(result);
        }

    }
}
