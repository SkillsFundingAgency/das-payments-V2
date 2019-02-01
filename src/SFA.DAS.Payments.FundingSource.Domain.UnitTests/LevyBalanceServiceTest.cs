using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Services;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class LevyBalanceServiceTest
    {
        [Test]
        public void TestInitialise()
        {
            var service = new LevyBalanceService();
            service.Initialise(100);
            var funded = service.TryFund(50);
            funded.Should().Be(50);
            funded = service.TryFund(100);
            funded.Should().Be(50);
            funded = service.TryFund(1);
            funded.Should().Be(0);
            funded = service.TryFund(-10);
            funded.Should().Be(-10);
            funded = service.TryFund(20);
            funded.Should().Be(10);
        }
    }
}
