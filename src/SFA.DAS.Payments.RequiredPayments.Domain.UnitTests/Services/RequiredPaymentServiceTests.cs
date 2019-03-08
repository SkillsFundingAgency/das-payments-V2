using System.Collections.Generic;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using Earning = SFA.DAS.Payments.Model.Core.OnProgramme.Earning;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class RequiredPaymentServiceTests
    {
        private RequiredPaymentService sut;
        private Mock<IRefundService> refundService;
        private Mock<IPaymentDueProcessor> paymentsDueService;


        [SetUp]
        public void Setup()
        {
            var automocker = AutoMock.GetStrict();
            paymentsDueService = automocker.Mock<IPaymentDueProcessor>();
            refundService = automocker.Mock<IRefundService>();
            sut = new RequiredPaymentService(paymentsDueService.Object, refundService.Object);
        }

        [Test]
        public void CallsPaymentsDueProcessor()
        {
            var testEarning = new Earning();
            var testHistory = new List<Payment>();

            var actual = sut.GetRequiredPayments(testEarning, testHistory);
        }

        [Test]
        public void DoesNotCallRefundServiceForGreaterThanZeroAmount()
        {

        }

        [Test]
        public void ReturnsTheResultOfPaymentsDueServiceForGreaterThanZeroAmount()
        {

        }

        [Test]
        public void CallsRefundServiceForLessThanZeroAmount()
        {

        }

        [Test]
        public void ReturnsTheResultOfTheRefundServiceForLessThanZeroAmount()
        {

        }

        [Test]
        public void ReutrnsAnEmptyListForZeroAmount()
        {

        }
    }
}
