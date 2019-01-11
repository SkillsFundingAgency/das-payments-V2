using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Payments.Model.Core.UnitTests.Tests.GivenADeliveryPeriod
{
    [TestFixture]
    public class WhenCallingCreateFromAcademicYearAndPeriod
    {
        [TestCase("1819", 1, 2018)]
        [TestCase("1819", 2, 2018)]
        [TestCase("1819", 3, 2018)]
        [TestCase("1819", 10, 2019)]
        [TestCase("1819", 11, 2019)]
        [TestCase("1819", 12, 2019)]
        [TestCase("1920", 1, 2019)]
        [TestCase("1920", 12, 2020)]
        public void YearIsCorrect(string academicYear, byte period, short expected)
        {
            var actual = DeliveryPeriod.CreateFromAcademicYearAndPeriod(academicYear, period);

            actual.Year.Should().Be(expected);
        }

        [TestCase("1819", 1, 8)]
        [TestCase("1819", 2, 9)]
        [TestCase("1819", 3, 10)]
        [TestCase("1819", 10, 5)]
        [TestCase("1819", 11, 6)]
        [TestCase("1819", 12, 7)]
        [TestCase("1920", 1, 8)]
        [TestCase("1920", 12, 7)]
        public void MonthIsCorrect(string academicYear, byte period, byte expected)
        {
            var actual = DeliveryPeriod.CreateFromAcademicYearAndPeriod(academicYear, period);

            actual.Month.Should().Be(expected);
        }

        [TestCase("1819", 1, "2018-08")]
        [TestCase("1819", 2, "2018-09")]
        [TestCase("1819", 3, "2018-10")]
        [TestCase("1819", 10, "2019-05")]
        [TestCase("1819", 11, "2019-06")]
        [TestCase("1819", 12, "2019-07")]
        [TestCase("1920", 1, "2019-08")]
        [TestCase("1920", 12, "2020-07")]
        public void IdentifierIsCorrect(string academicYear, byte period, string expected)
        {
            var actual = DeliveryPeriod.CreateFromAcademicYearAndPeriod(academicYear, period);

            actual.Identifier.Should().Be(expected);
        }

        [TestCase("1819", 1, 1)]
        [TestCase("1819", 2, 2)]
        [TestCase("1819", 3, 3)]
        [TestCase("1819", 10, 10)]
        [TestCase("1819", 11, 11)]
        [TestCase("1819", 12, 12)]
        [TestCase("1819", 13, 13)]
        [TestCase("1819", 14, 14)]
        [TestCase("1920", 1, 1)]
        [TestCase("1920", 12, 12)]
        [TestCase("1920", 14, 14)]
        public void PeriodIsCorrect(string academicYear, byte period, byte expected)
        {
            var actual = DeliveryPeriod.CreateFromAcademicYearAndPeriod(academicYear, period);

            actual.Period.Should().Be(expected);
        }
    }
}
