using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Factories;

namespace SFA.DAS.Payments.Model.Core.UnitTests.Tests.GivenACollectionPeriod
{
    [TestFixture]
    public class WhenCallingCreateFromAcademicYearAndPeriod
    {
        [TestCase(1617, 1, 1617)]
        [TestCase(1718, 1, 1718)]
        [TestCase(1819, 1, 1819)]
        [TestCase(1920, 1, 1920)]
        [TestCase(2021, 1, 2021)]
        [TestCase(2122, 1, 2122)]
        [TestCase(2223, 1, 2223)]
        [TestCase(2324, 1, 2324)]
        [TestCase(2425, 1, 2425)]
        [TestCase(2526, 1, 2526)]
        public void AcademicYearIsCorrect(short academicYear, byte period, short expected)
        {
            var actual = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(academicYear, period);

            actual.AcademicYear.Should().Be(expected);
        }

        [TestCase(1516, 1)] //before beginning of time
        [TestCase(2022, 1)] //two years gap
        [TestCase(2221, 1)] //invalid year order negative 1 year gap
        [TestCase(1924, 1)] //invalid year positive 4 years gap
        public void AcademicYearIsInvalid(short academicYear, byte period)
        {
            Func<CollectionPeriod> actual = () => CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(academicYear, period);

            actual.Should().Throw<ArgumentException>();
        }

        [TestCase(1819, 1, 1)]
        [TestCase(1819, 2, 2)]
        [TestCase(1819, 3, 3)]
        [TestCase(1819, 4, 4)]
        [TestCase(1819, 5, 5)]
        [TestCase(1819, 6, 6)]
        [TestCase(1819, 7, 7)]
        [TestCase(1819, 8, 8)]
        [TestCase(1920, 9, 9)]
        [TestCase(1920, 10, 10)]
        [TestCase(1920, 11, 11)]
        [TestCase(1920, 12, 12)]
        [TestCase(1920, 13, 13)]
        [TestCase(1920, 14, 14)]
        public void PeriodIsCorrect(short academicYear, byte period, byte expected)
        {
            var actual = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(academicYear, period);

            actual.Period.Should().Be(expected);
        }
    }
}
