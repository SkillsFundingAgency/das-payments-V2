using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.UnitTests.GivenAProviderAdjustmentCalculator
{
    public static class PaymentEntityTestExtensions
    {
        public static void ShouldContainPaymentMatchingEarning(this List<ProviderAdjustment> source, ProviderAdjustment earning)
        {
            source.Should().Contain(x => x.Ukprn == earning.Ukprn &&
                                         x.Amount == earning.Amount &&
                                         x.PaymentType == earning.PaymentType &&
                                         x.SubmissionId == earning.SubmissionId &&
                                         x.SubmissionCollectionPeriod == earning.SubmissionCollectionPeriod);
        }
    }

    [TestFixture]
    public class WhenCalculatingPayments
    {
        static void AssociatePaymentWithEarning(ProviderAdjustment payment, ProviderAdjustment earning)
        {
            payment.Ukprn = earning.Ukprn;
            payment.PaymentType = earning.PaymentType;
            payment.SubmissionCollectionPeriod = earning.SubmissionCollectionPeriod;
        }

        [TestFixture]
        public class WithNoPreviousPayments
        {
            [Test, AutoData]
            public void ThenTheTotalAmountPaidMatchesTheEarnings(
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testEarnings
                )
            {
                var actual = sut.CalculateDelta(new List<ProviderAdjustment>(), testEarnings, 0, 0);

                var expectedTotal = testEarnings.Sum(x => x.Amount);

                actual.Should().HaveCount(testEarnings.Count);
                actual.Sum(x => x.Amount).Should().Be(expectedTotal); 
            }

            [Test, AutoData]
            public void ThenThereAreMatchingPayments(
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testEarnings
            )
            {
                var actual = sut.CalculateDelta(new List<ProviderAdjustment>(), testEarnings, 0, 0).ToList();

                actual.ShouldContainPaymentMatchingEarning(testEarnings[0]);
                actual.ShouldContainPaymentMatchingEarning(testEarnings[1]);
                actual.ShouldContainPaymentMatchingEarning(testEarnings[2]);
            }

            [Test]
            [InlineAutoData(1, 2021, 8, 2020, "2021-R01")]
            [InlineAutoData(2, 2021, 9, 2020, "2021-R02")]
            [InlineAutoData(3, 2021, 10, 2020, "2021-R03")]
            [InlineAutoData(4, 2021, 11, 2020, "2021-R04")]
            [InlineAutoData(5, 2021, 12, 2020, "2021-R05")]
            [InlineAutoData(6, 2021, 1, 2021, "2021-R06")]
            [InlineAutoData(7, 2021, 2, 2021, "2021-R07")]
            [InlineAutoData(8, 2021, 3, 2021, "2021-R08")]
            [InlineAutoData(9, 2021, 4, 2021, "2021-R09")]
            [InlineAutoData(10, 2021, 5, 2021, "2021-R10")]
            [InlineAutoData(11, 2021, 6, 2021, "2021-R11")]
            [InlineAutoData(12, 2021, 7, 2021, "2021-R12")]
            [InlineAutoData(13, 2021, 9, 2021, "2021-R13")]
            [InlineAutoData(14, 2021, 10, 2021, "2021-R14")]
            [InlineAutoData(1, 2122, 8, 2021, "2122-R01")]
            [InlineAutoData(2, 2122, 9, 2021, "2122-R02")]
            [InlineAutoData(3, 2122, 10, 2021, "2122-R03")]
            [InlineAutoData(4, 2122, 11, 2021, "2122-R04")]
            [InlineAutoData(5, 2122, 12, 2021, "2122-R05")]
            [InlineAutoData(6, 2122, 1, 2022, "2122-R06")]
            [InlineAutoData(7, 2122, 2, 2022, "2122-R07")]
            [InlineAutoData(8, 2122, 3, 2022, "2122-R08")]
            [InlineAutoData(9, 2122, 4, 2022, "2122-R09")]
            [InlineAutoData(10, 2122, 5, 2022, "2122-R10")]
            [InlineAutoData(11, 2122, 6, 2022, "2122-R11")]
            [InlineAutoData(12, 2122, 7, 2022, "2122-R12")]
            [InlineAutoData(13, 2122, 9, 2022, "2122-R13")]
            [InlineAutoData(14, 2122, 10, 2022, "2122-R14")]
            public void ThenTheCollectionPeriodDetailsAreCalculatedCorrectly(int collectionPeriod, int academicYear, int expectedMonth, int expectedYear, string expectedName,
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testEarnings
            )
            {
                var actual = sut.CalculateDelta(new List<ProviderAdjustment>(), testEarnings, academicYear, collectionPeriod).ToList();

                actual.Select(x => x.CollectionPeriodMonth).Should().AllBeEquivalentTo(expectedMonth);
                actual.Select(x => x.CollectionPeriodYear).Should().AllBeEquivalentTo(expectedYear);
                Assert.That(actual.Select(x => x.CollectionPeriodName).All(x => x == expectedName));
            }
        }

        [TestFixture]
        public class WithOneSetOfPreviousPayments
        {
            [Test, AutoData]
            public void ThenTheTotalAmountPaidMatchesTheEarnings(
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testEarnings,
                List<ProviderAdjustment> testPreviousPayments
            )
            {
                for (var i = 0; i < 3; i++)
                {
                    AssociatePaymentWithEarning(testPreviousPayments[i], testEarnings[i]);
                }

                var actual = sut.CalculateDelta(testPreviousPayments, testEarnings, 0, 0);

                var expectedTotal = testEarnings.Sum(x => x.Amount) - testPreviousPayments.Sum(x => x.Amount);

                actual.Should().HaveCount(testEarnings.Count);
                actual.Sum(x => x.Amount).Should().Be(expectedTotal);
            }

            [Test, AutoData]
            public void ThenThereAreMatchingPayments(
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testEarnings,
                List<ProviderAdjustment> testPreviousPayments
            )
            {
                for (var i = 0; i < 3; i++)
                {
                    AssociatePaymentWithEarning(testPreviousPayments[i], testEarnings[i]);
                }

                var actual = sut.CalculateDelta(testPreviousPayments, testEarnings, 0, 0).ToList();

                testEarnings[0].Amount -= testPreviousPayments[0].Amount;
                testEarnings[1].Amount -= testPreviousPayments[1].Amount;
                testEarnings[2].Amount -= testPreviousPayments[2].Amount;

                actual.ShouldContainPaymentMatchingEarning(testEarnings[0]);
                actual.ShouldContainPaymentMatchingEarning(testEarnings[1]);
                actual.ShouldContainPaymentMatchingEarning(testEarnings[2]);
            }

            [Test, AutoData]
            public void AndThereIsNoChangeThereAreNoPayments(
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testEarnings,
                List<ProviderAdjustment> testPreviousPayments
            )
            {
                for (var i = 0; i < 3; i++)
                {
                    AssociatePaymentWithEarning(testPreviousPayments[i], testEarnings[i]);
                    testPreviousPayments[i].SubmissionId = testEarnings[i].SubmissionId;
                    testPreviousPayments[i].Amount = testEarnings[i].Amount;
                }

                var actual = sut.CalculateDelta(testPreviousPayments, testEarnings, 0,0);

                actual.Sum(x => x.Amount).Should().Be(0);
            }
        }

        [TestFixture]
        public class WithNoEarningsForPreviousPayments
        {
            [Test, AutoData]
            public void ThenTheTotalAmountPaidMatchesThePreviousPayments(
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testPreviousPayments
            )
            {
                var actual = sut.CalculateDelta(testPreviousPayments, new List<ProviderAdjustment>(), 0, 0);

                var expectedTotal = -1 * testPreviousPayments.Sum(x => x.Amount);

                actual.Should().HaveCount(testPreviousPayments.Count);
                actual.Sum(x => x.Amount).Should().Be(expectedTotal);
            }

            [Test, AutoData]
            public void ThenThereAreOppositePayments(
                ProviderAdjustmentCalculator sut,
                List<ProviderAdjustment> testPreviousPayments
            )
            {
                var actual = sut.CalculateDelta(testPreviousPayments, new List<ProviderAdjustment>(), 0, 0).ToList();

                testPreviousPayments[0].Amount *= -1;
                testPreviousPayments[1].Amount *= -1;
                testPreviousPayments[2].Amount *= -1;

                actual.ShouldContainPaymentMatchingEarning(testPreviousPayments[0]);
                actual.ShouldContainPaymentMatchingEarning(testPreviousPayments[1]);
                actual.ShouldContainPaymentMatchingEarning(testPreviousPayments[2]);
            }
        }
    }
}
