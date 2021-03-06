﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class CalculatedRequiredLevyAmountPrioritisationServiceTests
    {
        private CalculatedRequiredLevyAmountPrioritisationService calculatedRequiredLevyAmountPrioritisationService;

        [SetUp]
        public void SetUp()
        {
            calculatedRequiredLevyAmountPrioritisationService = new CalculatedRequiredLevyAmountPrioritisationService();
        }

        [Test]
        public void ShouldSortRefundsToBeFirst()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();

            var expectedSequence = new List<Guid>
            {
                expectedFirstEventId,
                expectedSecondEventId
            };

            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount { AmountDue = 1000, EventId = expectedSecondEventId, AgreedOnDate = DateTime.UtcNow, Learner = new Learner(){Uln = 1}, AccountId = 112 },
                new CalculatedRequiredLevyAmount { AmountDue = -1000, EventId = expectedFirstEventId, AccountId = 112 }
            };

            var result = calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, new List<(long, int)>());

           Assert.IsTrue( expectedSequence.SequenceEqual(result.Select(r => r.EventId)));
        }

        [Test]
        public void ShouldSortTransferToBeFirst()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();
            var expectedThirdEventId = Guid.NewGuid();
            var expectedFourthEventId = Guid.NewGuid();
            var expectedFifthEventId = Guid.NewGuid();

            var expectedSequence = new List<Guid>
            {
                expectedFirstEventId,
                expectedSecondEventId,
                expectedThirdEventId,
                expectedFourthEventId,
                expectedFifthEventId
            };

            var agreedOnDate = DateTime.UtcNow;
            var learner1 = new Learner {Uln = 10000001};
            var learner2 = new Learner {Uln = 10000002};

            //should be sorted by agreed on date then learner uln 
            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount {  EventId = expectedFifthEventId, TransferSenderAccountId = null, AgreedOnDate = agreedOnDate, Learner  = learner1, AccountId = 112 },
                new CalculatedRequiredLevyAmount {  EventId = expectedFirstEventId, TransferSenderAccountId = 100000001, AgreedOnDate = agreedOnDate, Learner  = learner1, AccountId = 112 },
                new CalculatedRequiredLevyAmount {  EventId = expectedThirdEventId, TransferSenderAccountId = 100000003,AgreedOnDate = agreedOnDate.AddMinutes(5), Learner  = learner1, AccountId = 112 },
                new CalculatedRequiredLevyAmount {  EventId = expectedSecondEventId, TransferSenderAccountId = 100000002,AgreedOnDate = agreedOnDate, Learner  = learner2, AccountId = 112 },
                new CalculatedRequiredLevyAmount {  EventId = expectedFourthEventId, TransferSenderAccountId = 100000005,AgreedOnDate = agreedOnDate.AddMinutes(5), Learner = learner2, AccountId = 112 }
            };

            var result = calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, new List<(long, int)>());

            Assert.IsTrue( expectedSequence.SequenceEqual(result.Select(r => r.EventId)));
        }

        [Test]
        public void ShouldSortStandardPaymentToBeLast()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();
            var expectedThirdEventId = Guid.NewGuid();

            var expectedSequence = new List<Guid>
            {
                expectedFirstEventId,
                expectedSecondEventId,
                expectedThirdEventId
            };

            var learner = new Learner {Uln = 10000002};
            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount { EventId = expectedThirdEventId, AmountDue = 1000, AgreedOnDate = DateTime.UtcNow, Learner  = new Learner(){Uln = 1}, AccountId = 112 },
                new CalculatedRequiredLevyAmount { EventId = expectedFirstEventId, AmountDue = -1000 , AgreedOnDate = DateTime.UtcNow, Learner  = new Learner(){Uln = 1}, AccountId = 112 },
                new CalculatedRequiredLevyAmount {  EventId = expectedSecondEventId, TransferSenderAccountId = 100000001, AgreedOnDate = DateTime.UtcNow, Learner  = learner, AccountId = 112 },
            };

            var result = calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, new List<(long, int)>());
            Assert.IsTrue( expectedSequence.SequenceEqual(result.Select(r => r.EventId)));
        }

        
        [Test]
        public void ShouldSortStandardPaymentsByProviderPriority()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();
            var expectedThirdEventId = Guid.NewGuid();

            var expectedSequence = new List<Guid>
            {
                expectedFirstEventId,
                expectedSecondEventId,
                expectedThirdEventId
            };
            var learner = new Learner {Uln = 10000002};
            var providerPriorities = new List<(long ukprn, int order)> {(1, 2), (2, 1), (3, 3)};
            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount { EventId = expectedSecondEventId, AmountDue = 1000, Ukprn = 1, AgreedOnDate = DateTime.UtcNow, Learner = learner, AccountId = 112 },
                new CalculatedRequiredLevyAmount { EventId = expectedFirstEventId, AmountDue = 1000 , Ukprn = 2, AgreedOnDate = DateTime.UtcNow, Learner = learner, AccountId = 112 },
                new CalculatedRequiredLevyAmount { EventId = expectedThirdEventId, AmountDue = 1000 , Ukprn = 3, AgreedOnDate = DateTime.UtcNow, Learner = learner, AccountId = 112 }
               };

            var result = calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, providerPriorities);
            Assert.IsTrue( expectedSequence.SequenceEqual(result.Select(r => r.EventId)));
        }


                
        [Test]
        public void ShouldSortUnprioritisedPaymentsAtBottomInCorrectOrder()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();
            var expectedThirdEventId = Guid.NewGuid();

            var expectedSequence = new List<Guid>
            {
                expectedFirstEventId,
                expectedSecondEventId,
                expectedThirdEventId
            };
            var learner = new Learner {Uln = 10000002};
            var providerPriorities = new List<(long ukprn, int order)> {(1, 2)};
            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount { EventId = expectedFirstEventId, AmountDue = 1000, Ukprn = 1, AgreedOnDate = DateTime.UtcNow, Learner = learner, AccountId = 112 },
                new CalculatedRequiredLevyAmount { EventId = expectedSecondEventId, AmountDue = 1000 , Ukprn = 3, AgreedOnDate = DateTime.UtcNow, Learner = learner, AccountId = 112 },
                new CalculatedRequiredLevyAmount { EventId = expectedThirdEventId, AmountDue = 1000 , Ukprn = 2, AgreedOnDate = DateTime.UtcNow.AddDays(1), Learner = learner, AccountId = 112 }
            };

            var result = calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, providerPriorities);
            Assert.IsTrue( expectedSequence.SequenceEqual(result.Select(r => r.EventId)));
        }


        [Test]
        public void ShouldSortWithinRefundsByTheOrderTheyCameIn()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();
            var expectedThirdEventId = Guid.NewGuid();

            var expectedSequence = new List<Guid>
            {
                expectedFirstEventId,
                expectedSecondEventId,
                expectedThirdEventId
            };

            var providerPriorities = new List<(long ukprn, int order)> { (1, 1), (2, 2) };
            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount { AmountDue = 1000, EventId = expectedThirdEventId, AgreedOnDate = DateTime.UtcNow, Learner = new Learner(){Uln = 1}, AccountId = 112 },
                new CalculatedRequiredLevyAmount { AmountDue = -1000, EventId = expectedFirstEventId, AccountId = 112, AgreedOnDate = DateTime.UtcNow.AddDays(1), Learner = new Learner{ Uln = 3 }, Ukprn = 2 },
                new CalculatedRequiredLevyAmount { AmountDue = -1000, EventId = expectedSecondEventId, AccountId = 112, AgreedOnDate = DateTime.UtcNow, Learner = new Learner{ Uln = 2 }, Ukprn = 1 }
            };

            var result = calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, providerPriorities);

            Assert.IsTrue(expectedSequence.SequenceEqual(result.Select(r => r.EventId)));
        }

        [Test]
        public void ShouldNotSortWithinTransfersByProviderPriority()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();
            var expectedThirdEventId = Guid.NewGuid();
            var expectedFourthEventId = Guid.NewGuid();
            var expectedFifthEventId = Guid.NewGuid();

            var expectedSequence = new List<Guid>
            {
                expectedFirstEventId,
                expectedSecondEventId,
                expectedThirdEventId,
                expectedFourthEventId,
                expectedFifthEventId
            };

            var agreedOnDate = DateTime.UtcNow;
            var learner1 = new Learner { Uln = 10000001 };
            var learner2 = new Learner { Uln = 10000002 };

            var providerPriorities = new List<(long ukprn, int order)> { (1, 1), (2, 2), (3, 3), (4, 4), (5, 5) };
            //should be sorted by agreed on date then learner uln 
            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount {  EventId = expectedFifthEventId, TransferSenderAccountId = null, AgreedOnDate = agreedOnDate, Learner  = learner1, AccountId = 112, Ukprn = 3 },
                new CalculatedRequiredLevyAmount {  EventId = expectedFirstEventId, TransferSenderAccountId = 100000001, AgreedOnDate = agreedOnDate, Learner  = learner1, AccountId = 112, Ukprn = 2 },
                new CalculatedRequiredLevyAmount {  EventId = expectedThirdEventId, TransferSenderAccountId = 100000003,AgreedOnDate = agreedOnDate.AddMinutes(5), Learner  = learner1, AccountId = 112, Ukprn = 4 },
                new CalculatedRequiredLevyAmount {  EventId = expectedSecondEventId, TransferSenderAccountId = 100000002,AgreedOnDate = agreedOnDate, Learner  = learner2, AccountId = 112, Ukprn = 1 },
                new CalculatedRequiredLevyAmount {  EventId = expectedFourthEventId, TransferSenderAccountId = 100000005,AgreedOnDate = agreedOnDate.AddMinutes(5), Learner = learner2, AccountId = 112, Ukprn = 5 }
            };

            var result = calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, providerPriorities);

            Assert.IsTrue(expectedSequence.SequenceEqual(result.Select(r => r.EventId)));
        }
    }
}