using System;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class StartDateValidatorTests
    {
        [Test]
        public void ReturnsValidApprenticeships()
        {
            var startDate = DateTime.UtcNow;
            var earningPeriod = new PriceEpisode {StartDate = startDate};
        }
    }
}
