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
        private IlrSubmittedEvent currentIlr;

        [SetUp]
        public void Setup()
        {

            currentIlr = new IlrSubmittedEvent
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
                IncomingPaymentJobId = currentIlr.JobId,
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
                IncomingPaymentJobId = currentIlr.JobId,
                IncomingPaymentSubmissionDate = DateTime.MinValue
            });

            Assert.IsFalse(result);
        }

        [Test]
        public void IsNewIlrSubmissionShouldBeValidIfCurrentIlrIsNull()
        {
            validateIlrSubmission = new ValidateIlrSubmission();

            var result = validateIlrSubmission.IsNewIlrSubmission(new IlrSubmissionValidationRequest
            {
                CurrentIlr = null
            });

            Assert.IsTrue(result);
        }

        [Test]
        public void IsNewIlrSubmissionShouldBeValidIfCurrentIlrDoNotMatchIncomingMessageIlrDate()
        {
            validateIlrSubmission = new ValidateIlrSubmission();

            var result = validateIlrSubmission.IsNewIlrSubmission(new IlrSubmissionValidationRequest
            {
                CurrentIlr = currentIlr,
                IncomingPaymentUkprn = currentIlr.Ukprn,
                IncomingPaymentJobId = currentIlr.JobId,
                IncomingPaymentSubmissionDate = DateTime.MinValue
            });

            Assert.IsTrue(result);
        }


        [Test]
        public void IsNewIlrSubmissionShouldBeInValidIfCurrentIlrMatchIncomingMessageIlr()
        {
            validateIlrSubmission = new ValidateIlrSubmission();
            var result = validateIlrSubmission.IsNewIlrSubmission(new IlrSubmissionValidationRequest
            {
                CurrentIlr = currentIlr,
                IncomingPaymentUkprn = currentIlr.Ukprn,
                IncomingPaymentJobId = currentIlr.JobId,
                IncomingPaymentSubmissionDate = currentIlr.IlrSubmissionDateTime
            });
            Assert.IsFalse(result);
        }


    }
}
