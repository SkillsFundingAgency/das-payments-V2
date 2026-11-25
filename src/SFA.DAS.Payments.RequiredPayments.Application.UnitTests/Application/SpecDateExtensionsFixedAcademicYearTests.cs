using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application
{
    [TestFixture]
    public class SpecDateExtensionsFixedAcademicYearTests
    {
        [TestCase("R01/2025-26 Academic Year", "2025-08-01")]
        [TestCase("R05/2025-26 Academic Year", "2025-12-01")]
        [TestCase("R06/2025-26 Academic Year", "2026-01-01")]
        [TestCase("R12/2025-26 Academic Year", "2026-07-01")]
        [TestCase("r09/2025-26 academic year", "2026-04-01")]
        [TestCase("Aug/2025-26 Academic Year", "2025-08-01")]
        [TestCase("Jan/2025-26 Academic Year", "2026-01-01")]
        [TestCase("Jul/2026-27 Academic Year", "2027-07-01")]
        [TestCase("R03/2024-25 Academic Year", "2024-10-01")]
        public void ToDate_Maps_Fixed_Academic_Year_Period_Or_Month_To_First_Of_Month(string text, string expected)
        {
            text.ToDate().Should().Be(DateTime.Parse(expected));
        }

        [TestCase("01/Aug/2025-26 Academic Year", "2025-08-01")]
        [TestCase("06/Aug/2025-26 Academic Year", "2025-08-06")]
        [TestCase("31/Jul/2025-26 Academic Year", "2026-07-31")]
        [TestCase("08/Sep/2026-27 Academic Year", "2026-09-08")]
        public void ToDate_Maps_Fixed_Academic_Year_With_Day_To_Exact_Date(string text, string expected)
        {
            text.ToDate().Should().Be(DateTime.Parse(expected));
        }

        [TestCase("R01/2025-27 Academic Year")]
        [TestCase("R01/2025-25 Academic Year")]
        [TestCase("01/Aug/2025-24 Academic Year")]
        public void ToDate_Throws_When_Fixed_Academic_Year_Is_Not_Consecutive(string text)
        {
            Action act = () => text.ToDate();

            act.Should().Throw<Exception>().WithMessage("*second year must follow the first*");
        }

        [TestCase("2025-26 Academic Year", "2526")]
        [TestCase("2026-27 Academic Year", "2627")]
        [TestCase("2024-25 academic year", "2425")]
        public void ToAcademicYear_Maps_Fixed_Academic_Year_To_Four_Digit_Code(string text, string expected)
        {
            text.ToAcademicYear().Should().Be(expected);
        }

        [Test]
        public void ToAcademicYear_Throws_When_Fixed_Academic_Year_Is_Not_Consecutive()
        {
            Action act = () => "2025-27 Academic Year".ToAcademicYear();

            act.Should().Throw<Exception>().WithMessage("*second year must follow the first*");
        }
    }
}
