


using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.UnitTests
{
    [TestFixture]
    public class WhenTestingHashCode
    {
        private static ProviderAdjustment earning1 = new ProviderAdjustment
        {
            Amount = 10,
            CollectionPeriodMonth = 11,
            CollectionPeriodName = "123",
            CollectionPeriodYear = 13,
            PaymentType = 14,
            PaymentTypeName = "15",
            SubmissionAcademicYear = 16,
            SubmissionCollectionPeriod = 17,
            SubmissionId = Guid.NewGuid(),
            Ukprn = 18,
        };

        private static ProviderAdjustment earning2 = new ProviderAdjustment
        {
            Amount = 20,
            CollectionPeriodMonth = 21,
            CollectionPeriodName = "223",
            CollectionPeriodYear = 23,
            PaymentType = 24,
            PaymentTypeName = "25",
            SubmissionAcademicYear = 26,
            SubmissionCollectionPeriod = 27,
            SubmissionId = Guid.NewGuid(),
            Ukprn = 28,
        };

        [Test]
        public void WithDifferringUkprnAreNotEqual()
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test]
        public void WithDifferringPaymentTypeAreNotEqual()
        {
            earning2.Ukprn = earning1.Ukprn;
            earning2.SubmissionCollectionPeriod = earning1.SubmissionCollectionPeriod;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test]
        public void WithDifferringPeriodAreNotEqual()
        {
            earning2.PaymentType = earning1.PaymentType;
            earning2.Ukprn = earning1.Ukprn;

            var sut = new ProviderAdjustmentPaymentGrouping(earning1);
            var test = new ProviderAdjustmentPaymentGrouping(earning2);

            var actual = sut.GetHashCode() == test.GetHashCode();

            actual.Should().BeFalse();
        }

        [Test]
        public void WithSameUkprnPaymentTypeAndPeriodAreSame()
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
