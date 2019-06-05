using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using FluentAssertions;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class CalculatePeriodStartAndEndDateTest
    {

        [TestCase( 1, 1920, "2019/8/1","2019/8/31")]
        [TestCase( 2, 1920, "2019/9/1", "2019/9/30")]
        [TestCase(5, 1920, "2019/12/1", "2019/12/31")]
        [TestCase(6, 1920, "2020/1/1","2020/1/31")]
        [TestCase( 7, 1920, "2020/2/1", "2020/2/29")]
        [TestCase(11, 1920, "2020/6/1", "2020/6/30")]
        [TestCase(12, 1920, "2020/7/1", "2020/7/31")]
        [TestCase(1, 1819, "2018/8/1", "2018/8/31")]
        public void ShouldCalculatePeriodStartAndEndDate(byte testPeriod, short academicYear, DateTime expectedPeriodStartDate ,DateTime expectedPeriodEndDate)
        {
            var calculatePeriodStartAndEndDate = new CalculatePeriodStartAndEndDate();
            var actualResult = calculatePeriodStartAndEndDate.GetPeriodDate(testPeriod, academicYear);
            actualResult.periodStartDate.Should().Be(expectedPeriodStartDate);
            actualResult.periodEndDate.Should().Be(expectedPeriodEndDate);
        }
        
    }
}


