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
        private ValidateIlrSubmission validateIlrSubmission;

        [Test]
        public void ShouldBeValidIfCurrentIlrIsNull()
        {
            validateIlrSubmission = new ValidateIlrSubmission();

            var result = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
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

            validateIlrSubmission = new ValidateIlrSubmission();

            var result = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
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

            validateIlrSubmission = new ValidateIlrSubmission();

            var result = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
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
