using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.UnitTests.GivenAProviderAdjustmentCalculator
{
    [TestFixture]
    public class WhenPopulatingCollectionPeriodForPayments
    {
        [Test]
        [InlineAutoData(1, 8)]
        [InlineAutoData(2, 9)]
        [InlineAutoData(3, 10)]
        [InlineAutoData(4, 11)]
        [InlineAutoData(5, 12)]
        [InlineAutoData(6, 1)]
        [InlineAutoData(7, 2)]
        [InlineAutoData(8, 3)]
        [InlineAutoData(9, 4)]
        [InlineAutoData(10, 5)]
        [InlineAutoData(11, 6)]
        [InlineAutoData(12, 7)]
        [InlineAutoData(13, 9)]
        [InlineAutoData(14, 10)]
        public void ThenTheCollectionPeriodMonth_Should_BeTheExpectedValue(int collectionPeriod, int expectedMonth,
            ProviderAdjustmentCalculator sut,
            List<ProviderAdjustment> testPayments
        )
        {
            sut.PopulateCollectonPeriodForPayments(testPayments, 2021, collectionPeriod);

            testPayments.Should().AllBeEquivalentTo(new {CollectionPeriodMonth = expectedMonth});
        }
    }
}
