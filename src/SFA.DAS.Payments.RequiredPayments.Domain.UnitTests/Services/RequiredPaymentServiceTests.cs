using NUnit.Framework;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class RequiredPaymentServiceTests
    {
        [Test]
        public void CallsPaymentsDueProcessor()
        {
            var sut = new RequiredPaymentService();

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
