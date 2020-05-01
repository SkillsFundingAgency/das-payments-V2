using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Services;
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
        public async Task ShouldSortRefundsToBeFirst()
        {
            var expectedFirstEventId = Guid.NewGuid();
            var expectedSecondEventId = Guid.NewGuid();

            var amounts = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount { AmountDue = 1000, EventId = expectedSecondEventId },
                new CalculatedRequiredLevyAmount { AmountDue = -1000, EventId = expectedFirstEventId }
            };

            var result = await calculatedRequiredLevyAmountPrioritisationService.Prioritise(amounts, new List<(long, int)>());

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().EventId, Is.EqualTo(expectedFirstEventId));
            Assert.That(result.Last().EventId, Is.EqualTo(expectedSecondEventId));
        }
    }
}