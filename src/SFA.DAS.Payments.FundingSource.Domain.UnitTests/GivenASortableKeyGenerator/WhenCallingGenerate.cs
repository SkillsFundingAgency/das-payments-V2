using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests.GivenASortableKeyGenerator
{
    [TestFixture]
    public class WhenCallingGenerate
    {
        protected static ISortableKeyGenerator sut;
        private DateTime startDate;
        protected static Guid eventId;

        [SetUp]
        public void Setup()
        {
            sut = new SortableKeyGenerator();
            startDate = DateTime.Now;
            eventId = Guid.NewGuid();
        }

        [Test]
        public void ThenTheResultsShouldSortRefundsBeforePayments()
        {
            var keys = new List<string>();
            var paymentKey = sut.Generate(1, 0, 0, startDate, false, eventId);
            keys.Add(paymentKey);
            var refundKey = sut.Generate(-100, 0, 0, startDate, false, eventId);
            keys.Add(refundKey);

            keys.Sort();

            keys[0].Should().Be(refundKey);
            keys[1].Should().Be(paymentKey);
        }
        [Test]
        public void ThenTheResultsShouldSortTransfersBeforePayments()
        {
            var keys = new List<string>();
            var paymentKey = sut.Generate(1, 0, 0, startDate, false, eventId);
            keys.Add(paymentKey);
            var xferKey = sut.Generate(1, 0, 0, startDate, true, eventId);
            keys.Add(xferKey);

            keys.Sort();

            keys[0].Should().Be(xferKey);
            keys[1].Should().Be(paymentKey);
        }
        [Test]
        public void ThenTheResultsShouldSortByrPriority()
        {
            var keys = new List<string>();
            var expectedFirst = sut.Generate(1, 1, 0, startDate, false, eventId);
            keys.Add(expectedFirst);
            var expectedSecond = sut.Generate(1, 2, 0, startDate, false, eventId);
            keys.Add(expectedSecond);

            keys.Sort();

            keys[0].Should().Be(expectedFirst);
            keys[1].Should().Be(expectedSecond);
        }

        [Test]
        public void ThenTheResultsShouldSortByDateAgreed()
        {
            var keys = new List<string>();
            var expectedFirst = sut.Generate(1, 1, 0, startDate, false, eventId);
            keys.Add(expectedFirst);
            var expectedSecond = sut.Generate(1, 1, 0, startDate.AddDays(1), false, eventId);
            keys.Add(expectedSecond);

            keys.Sort();

            keys[0].Should().Be(expectedFirst);
            keys[1].Should().Be(expectedSecond);
        }

        [Test]
        public void ThenTheResultsShouldSortByUln()
        {
            var keys = new List<string>();
            var expectedFirst = sut.Generate(1, 1, 10, startDate, false, eventId);
            keys.Add(expectedFirst);
            var expectedSecond = sut.Generate(1, 1, 100, startDate, false, Guid.NewGuid());
            keys.Add(expectedSecond);

            keys.Sort();

            keys[0].Should().Be(expectedFirst);
            keys[1].Should().Be(expectedSecond);
        }

        [TestFixture]
        public class AndAllEventsArePayments
        {
            [Test]
            public new void ThenTheResultsShouldSortByPriority()
            {
                var keys = new List<string>();
                var lowPriorityKey = sut.Generate(100, 2, 0, DateTime.Today, false, eventId);
                keys.Add(lowPriorityKey);
                var highPriorityKey = sut.Generate(1, 1, 0, DateTime.Today, false, eventId);
                keys.Add(highPriorityKey);

                keys.Sort();

                keys[0].Should().Be(highPriorityKey);
                keys[1].Should().Be(lowPriorityKey);
            }
        }

        [TestFixture]
        public class AndAllEventsAreRefunds
        {
            [Test]
            public new void ThenTheResultsShouldSortByPriority()
            {
                var keys = new List<string>();
                var lowPriorityKey = sut.Generate(-1, 2, 0, DateTime.Today, false, eventId);
                keys.Add(lowPriorityKey);
                var highPriorityKey = sut.Generate(-100, 1, 0, DateTime.Today, false, eventId);
                keys.Add(highPriorityKey);

                keys.Sort();

                keys[0].Should().Be(highPriorityKey);
                keys[1].Should().Be(lowPriorityKey);
            }
        }
    }
}
