using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests.GivenASortableKeyGenerator
{
    [TestFixture]
    public class WhenCallingGenerate
    {
        protected static IGenerateSortableKeys sut;

        [SetUp]
        public void Setup()
        {
            sut = new SortableKeyGenerator();
        }

        [Test]
        public void ThenTheResultsShouldSortRefundsBeforePayments()
        {
            var refundEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                AmountDue = -100,
                Learner = new Learner(),
            };

            var paymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                AmountDue = 1,
                Learner = new Learner(),
            };

            var keys = new List<string>();
            var paymentKey = sut.Generate(paymentEvent);
            keys.Add(paymentKey);
            var refundKey = sut.Generate(refundEvent);
            keys.Add(refundKey);

            keys.Sort();

            keys[0].Should().Be(refundKey);
            keys[1].Should().Be(paymentKey);
        }

        [TestFixture]
        public class AndAllEventsArePayments 
        {
            [Test]
            public void ThenTheResultsShouldSortByPriority()
            {
                var highPriorityEvent = new ApprenticeshipContractType1RequiredPaymentEvent
                {
                    AmountDue = 1,
                    Priority = 1,
                    Learner = new Learner(),
                };

                var lowPriorityEvent = new ApprenticeshipContractType1RequiredPaymentEvent
                {
                    AmountDue = 100,
                    Priority = 2,
                    Learner = new Learner(),
                };

                var keys = new List<string>();
                var lowPriorityKey = sut.Generate(lowPriorityEvent);
                keys.Add(lowPriorityKey);
                var highPriorityKey = sut.Generate(highPriorityEvent);
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
            public void ThenTheResultsShouldSortByPriority()
            {
                var highPriorityEvent = new ApprenticeshipContractType1RequiredPaymentEvent
                {
                    AmountDue = -1,
                    Priority = 1,
                    Learner = new Learner(),
                };

                var lowPriorityEvent = new ApprenticeshipContractType1RequiredPaymentEvent
                {
                    AmountDue = -100,
                    Priority = 2,
                    Learner = new Learner(),
                };

                var keys = new List<string>();
                var lowPriorityKey = sut.Generate(lowPriorityEvent);
                keys.Add(lowPriorityKey);
                var highPriorityKey = sut.Generate(highPriorityEvent);
                keys.Add(highPriorityKey);

                keys.Sort();

                keys[0].Should().Be(highPriorityKey);
                keys[1].Should().Be(lowPriorityKey);
            }
        }
    }
}
