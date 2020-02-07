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
                var actual = sut.CalculateDelta(new List<ProviderAdjustment>(), testEarnings);

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
                var actual = sut.CalculateDelta(new List<ProviderAdjustment>(), testEarnings).ToList();

                actual.ShouldContainPaymentMatchingEarning(testEarnings[0]);
                actual.ShouldContainPaymentMatchingEarning(testEarnings[1]);
                actual.ShouldContainPaymentMatchingEarning(testEarnings[2]);
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

                var actual = sut.CalculateDelta(testPreviousPayments, testEarnings);

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

                var actual = sut.CalculateDelta(testPreviousPayments, testEarnings).ToList();

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

                var actual = sut.CalculateDelta(testPreviousPayments, testEarnings);

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
                var actual = sut.CalculateDelta(testPreviousPayments, new List<ProviderAdjustment>());

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
                var actual = sut.CalculateDelta(testPreviousPayments, new List<ProviderAdjustment>()).ToList();

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
