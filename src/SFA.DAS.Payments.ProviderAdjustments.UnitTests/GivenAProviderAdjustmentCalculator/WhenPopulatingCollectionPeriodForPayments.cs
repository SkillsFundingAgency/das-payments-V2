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
        [InlineAutoData(1, 8, 2021)]
        [InlineAutoData(2, 9, 2021)]
        [InlineAutoData(3, 10, 2021)]
        [InlineAutoData(4, 11, 2021)]
        [InlineAutoData(5, 12, 2021)]
        [InlineAutoData(6, 1, 2021)]
        [InlineAutoData(7, 2, 2021)]
        [InlineAutoData(8, 3, 2021)]
        [InlineAutoData(9, 4, 2021)]
        [InlineAutoData(10, 5, 2021)]
        [InlineAutoData(11, 6, 2021)]
        [InlineAutoData(12, 7, 2021)]
        [InlineAutoData(13, 9, 2021)]
        [InlineAutoData(14, 10, 2021)]
        [InlineAutoData(1, 8, 2122)]
        [InlineAutoData(2, 9, 2122)]
        [InlineAutoData(3, 10, 2122)]
        [InlineAutoData(4, 11, 2122)]
        [InlineAutoData(5, 12, 2122)]
        [InlineAutoData(6, 1, 2122)]
        [InlineAutoData(7, 2, 2122)]
        [InlineAutoData(8, 3, 2122)]
        [InlineAutoData(9, 4, 2122)]
        [InlineAutoData(10, 5, 2122)]
        [InlineAutoData(11, 6, 2122)]
        [InlineAutoData(12, 7, 2122)]
        [InlineAutoData(13, 9, 2122)]
        [InlineAutoData(14, 10, 2122)]
        public void ThenTheCollectionPeriodMonth_Should_BeTheExpectedValue(int collectionPeriod, int expectedMonth, int academicYear,
            ProviderAdjustmentCalculator sut,
            List<ProviderAdjustment> testPayments
        )
        {
            sut.PopulateCollectonPeriodForPayments(testPayments, academicYear, collectionPeriod);

            testPayments.Should().AllBeEquivalentTo(new {CollectionPeriodMonth = expectedMonth});
        }
    }
}
