using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Messages.UnitTests
{
    [TestFixture]
    public class CalendarPeriodTest
    {
        [Test]
        [TestCase("1819R01")]
        [TestCase("1819-R01")]
        public void TestNameToPeriod1(string name)
        {
            var period = new CalendarPeriod(name);
            Assert.AreEqual(2018, period.Year);
            Assert.AreEqual(8, period.Month);
            Assert.AreEqual(1, period.Period);
        }

        [Test]
        [TestCase("1819R05")]
        [TestCase("1819-R05")]
        public void TestNameToPeriod5(string name)
        {
            var period = new CalendarPeriod(name);
            Assert.AreEqual(2018, period.Year);
            Assert.AreEqual(12, period.Month);
            Assert.AreEqual(5, period.Period);
        }

        [Test]
        [TestCase("1819R06")]
        [TestCase("1819-R06")]
        public void TestNameToPeriod6(string name)
        {
            var period = new CalendarPeriod(name);
            Assert.AreEqual(2019, period.Year);
            Assert.AreEqual(1, period.Month);
            Assert.AreEqual(6, period.Period);
        }

        [Test]
        [TestCase("1819R12")]
        [TestCase("1819-R12")]
        public void TestNameToPeriod12(string name)
        {
            var period = new CalendarPeriod(name);
            Assert.AreEqual(2019, period.Year);
            Assert.AreEqual(7, period.Month);
            Assert.AreEqual(12, period.Period);
        }

        [Test]
        [TestCase("1819R14")]
        [TestCase("1819-R14")]
        public void TestNameToPeriod14(string name)
        {
            var period = new CalendarPeriod(name);
            Assert.AreEqual(2019, period.Year);
            Assert.AreEqual(14, period.Period);
        }

        [Test]
        public void TestInvalidNameNull()
        {
            try
            {
                new CalendarPeriod(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("name", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [TestCase("")]
        [TestCase("1819R1")]
        [TestCase("1819R111")]
        [TestCase("1819R00")]
        [TestCase("1819R15")]
        [TestCase("1i19R01")]
        [TestCase("1819-R1")]
        [TestCase("1819-R111")]
        [TestCase("1819-R00")]
        [TestCase("1819-R15")]
        [TestCase("1i19-R01")]
        public void TestInvalidName(string name)
        {
            try
            {
                new CalendarPeriod(name);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("name", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [TestCase(2018, 8, "1819-R01", 1)]
        [TestCase(2018, 12, "1819-R05", 5)]
        [TestCase(2019, 1, "1819-R06", 6)]
        [TestCase(2019, 7, "1819-R12", 12)]
        public void TestYearMonthConstructor(short year, byte month, string expectedName, byte expectedPeriod)
        {
            var period = new CalendarPeriod(year, month);
            Assert.AreEqual(expectedName, period.Name);
            Assert.AreEqual(expectedPeriod, period.Period);
        }

        [TestCase(2018, 8, "1819", 1)]
        [TestCase(2018, 12, "1819", 5)]
        [TestCase(2019, 1, "1819", 6)]
        [TestCase(2019, 7, "1819", 12)]
        public void TestPeriodConstructor(short expectedYear, byte expectedMonth, string years, byte period)
        {
            var subj = new CalendarPeriod(years, period);
            Assert.AreEqual(expectedYear, subj.Year);
            Assert.AreEqual(expectedMonth, subj.Month);
        }

        [Test]
        public void TestEquals()
        {
            Assert.IsTrue(new CalendarPeriod("1819-R01") == new CalendarPeriod("1819-R01"));
            Assert.IsTrue(new CalendarPeriod("1819-R01") == new CalendarPeriod(2018, 8));
            Assert.IsTrue(new CalendarPeriod("1819-R01") == new CalendarPeriod("1819", 1));
        }
    }
}
