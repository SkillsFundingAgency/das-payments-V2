

using NUnit.Framework;

namespace SFA.DAS.Payments.Calc.ProviderAdjustments.UnitTests.DomainTests.GivenAProviderPaymentsGroup
{
    [TestFixture]
    public class WhenTestingHashCode
    {
        [Test]
        [AutoData]
        public void WithDifferringUkprnAreNotEqual(
            AdjustmentEntity earning1,
            AdjustmentEntity earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderPaymentsGroup(earning1);
            var test = new ProviderPaymentsGroup(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test]
        [AutoData]
        public void WithDifferringPaymentTypeAreNotEqual(
            AdjustmentEntity earning1,
            AdjustmentEntity earning2)
        {
            earning2.Ukprn = earning1.Ukprn;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderPaymentsGroup(earning1);
            var test = new ProviderPaymentsGroup(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test]
        [AutoData]
        public void WithDifferringPeriodAreNotEqual(
            AdjustmentEntity earning1,
            AdjustmentEntity earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderPaymentsGroup(earning1);
            var test = new ProviderPaymentsGroup(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test]
        [AutoData]
        public void WithSameUkprnPaymentTypeAndPeriodAreSame(
            AdjustmentEntity earning1,
            AdjustmentEntity earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderPaymentsGroup(earning1);
            var test = new ProviderPaymentsGroup(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeTrue();
        }
    }
}
