using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.UnitTests
{
    [TestFixture]
    public class WhenTestingForEquality
    {
        [Test]
        [AutoData]
        public void WithDifferringUkprnAreNotEqual(
            ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut == test;

            actual.Should().BeFalse();
            earning1.Ukprn.Should().NotBe(earning2.Ukprn);
        }

        [Test]
        [AutoData]
        public void WithDifferringPaymentTypeAreNotEqual(
            ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.Ukprn = earning1.Ukprn;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut == test;

            actual.Should().BeFalse();
            earning1.PaymentType.Should().NotBe(earning2.PaymentType);
        }

        [Test]
        [AutoData]
        public void WithDifferringPeriodAreNotEqual(
            ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut == test;

            actual.Should().BeFalse();
            earning1.SubmissionCollectionPeriod.Should().NotBe(earning2.SubmissionCollectionPeriod);
        }

        [Test]
        [AutoData]
        public void WithSameUkprnPaymentTypeAndPeriodAreSame(
            ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut == test;

            actual.Should().BeTrue();
        }

        [Test]
        [AutoData]
        public void WithDifferninSubmissionIdSame(
            ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut == test;

            actual.Should().BeTrue();
            earning1.SubmissionId.Should().NotBe(earning2.SubmissionId);
        }

        [Test]
        [AutoData]
        public void WithDifferninPaymentTypeNameSame(
            ProviderAdjustment earning1,
            ProviderAdjustment earning2)
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut == test;

            actual.Should().BeTrue();
            earning1.PaymentTypeName.Should().NotBe(earning2.PaymentTypeName);
        }
    }
}
