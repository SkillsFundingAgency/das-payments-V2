using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.UnitTests
{
    [TestFixture]
    public class WhenTestingHashCode
    {
        [Test, AutoData]
        public void WithDifferringUkprnAreNotEqual(ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test, AutoData]
        public void WithDifferringPaymentTypeAreNotEqual(ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.Ukprn = earning1.Ukprn;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test, AutoData]
        public void WithDifferringPeriodAreNotEqual(ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test, AutoData]
        public void WithSameUkprnPaymentTypeAndPeriodAreSame(ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeTrue();
        }
    }
}
