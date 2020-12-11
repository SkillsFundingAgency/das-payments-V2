using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;

namespace SFA.DAS.Payments.ProviderPayments.Domain.UnitTests
{
    [TestFixture]
    public class ValidatePaymentMessageTest
    {
        private ValidateIlrSubmission validateIlrSubmission;
        private ReceivedProviderEarningsEvent currentIlr;

        [SetUp]
        public void Setup()
        {

            currentIlr = new ReceivedProviderEarningsEvent
            {
                Ukprn = 1000,
                JobId = 1000,
                IlrSubmissionDateTime = DateTime.MaxValue
            };
        }

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
        public void IsLatestIlrPaymentShouldBeValidIfCurrentIlrMatchesIncomingMessage()
        {

            validateIlrSubmission = new ValidateIlrSubmission();

            var result = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
            {
                CurrentIlr = currentIlr,
                IncomingPaymentUkprn = currentIlr.Ukprn,
                IncomingPaymentSubmissionDate = DateTime.MaxValue
            });

            Assert.IsTrue(result);
        }

        [Test]
        public void IsLatestIlrPaymentShouldBeInValidIfCurrentIlrDoNotMatchIncomingMessage()
        {
            validateIlrSubmission = new ValidateIlrSubmission();

            var result = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
            {
                CurrentIlr = currentIlr,
                IncomingPaymentUkprn = currentIlr.Ukprn,
                IncomingPaymentSubmissionDate = DateTime.MinValue
            });

            Assert.IsFalse(result);
        }
    }
}
