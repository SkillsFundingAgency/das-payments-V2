using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Messages
{
    [TestFixture]
    public class NamedCalendarPeriodTest
    {
        [Test]
        public void TestNameToPeriod1()
        {
            var period = NamedCalendarPeriod.FromName("1819R01");
            Assert.AreEqual(2018, period.Year);
            Assert.AreEqual(8, period.Month);
        }

        [Test]
        public void TestNameToPeriod5()
        {
            var period = NamedCalendarPeriod.FromName("1819R01");
            Assert.AreEqual(2018, period.Year);
            Assert.AreEqual(8, period.Month);
        }

        public void TestNameToPeriod6()
        {
            var period = NamedCalendarPeriod.FromName("1819R01");
            Assert.AreEqual(2018, period.Year);
            Assert.AreEqual(8, period.Month);
        }

        [Test]
        public void TestNameToPeriod12()
        {
            var period = NamedCalendarPeriod.FromName("1819R01");
            Assert.AreEqual(2018, period.Year);
            Assert.AreEqual(8, period.Month);
        }

        [Test]
        public void TestNameToPeriod14()
        {
            var period = NamedCalendarPeriod.FromName("1819R01");
            Assert.AreEqual(2018, period.Year);
            Assert.AreEqual(8, period.Month);
        }

        [Test]
        public void TestInvalidNameNull()
        {
            try
            {
                NamedCalendarPeriod.FromName(null);
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
        public void TestInvalidName(string name)
        {
            try
            {
                NamedCalendarPeriod.FromName(name);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("name", ex.ParamName);
                return;
            }

            Assert.Fail();
        }
    }
}
