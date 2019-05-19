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
            service.Initialise(100,50);
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

        [Test]
        public void Should_Allow_Transfer_Payments_If_Enough_Allowance_And_Levy()
        {
            var service = new LevyBalanceService();
            service.Initialise(100,50);
            service.TryFundTransfer(50).Should().Be(50);
        }

        [Test]
        public void Should_Allow_Partial_Transfer_Funding_If_Enough_Levy_But_Not_Enough_Allowance_To_Cover_Whole_Amount()
        {
            var service = new LevyBalanceService();
            service.Initialise(100, 50);
            service.TryFundTransfer(75).Should().Be(50);
        }

        [Test]
        public void Should_Allow_Partial_Transfer_Funding_If_Not_Enough_Levy_To_Cover_Whole_Amount()
        {
            var service = new LevyBalanceService();
            service.Initialise(50, 75);
            service.TryFundTransfer(75).Should().Be(50);
        }

        [Test]
        public void Should_Not_Allow_Transfer_Funding_If_Enough_Levy_But_No_Allowance()
        {
            var service = new LevyBalanceService();
            service.Initialise(50, 0);
            service.TryFundTransfer(50).Should().Be(0);
        }

        [Test]
        public void Should_Not_Allow_Transfer_Funding_If_Enough_Levy_But_No_Remaining_Allowance()
        {
            var service = new LevyBalanceService();
            service.Initialise(75, 50);
            service.TryFundTransfer(50).Should().Be(50);
            service.TryFundTransfer(25).Should().Be(0);
        }

        [Test]
        public void Should_Allow_Levy_Funding_After_Transfer_Funds_Have_Been_Spent()
        {
            var service = new LevyBalanceService();
            service.Initialise(75, 50);
            service.TryFundTransfer(50).Should().Be(50);
            service.TryFund(25).Should().Be(25);
        }

        [Test]
        public void Should_Not_Allow_Levy_Funding_If_All_Levy_Spent_On_Transfers()
        {
            var service = new LevyBalanceService();
            service.Initialise(75, 75);
            service.TryFundTransfer(50).Should().Be(50);
            service.TryFundTransfer(25).Should().Be(25);
            service.TryFund(25).Should().Be(0);
        }

        [Test]
        public void Should_Return_Transfer_Refund_Amount()
        {
            var service = new LevyBalanceService();
            service.Initialise(75, 0);
            service.TryFundTransfer(-50).Should().Be(-50);
        }

        [Test]
        public void Should_Credit_Transfer_Refunds_To_Transfer_Allowance()
        {
            var service = new LevyBalanceService();
            service.Initialise(75, 0);
            service.TryFundTransfer(-50).Should().Be(-50);
            service.TryFundTransfer(50).Should().Be(50);
        }

        [Test]
        public void Should_Credit_Transfer_Refunds_To_Levy_Balance()
        {
            var service = new LevyBalanceService();
            service.Initialise(0, 0);
            service.TryFundTransfer(-50).Should().Be(-50);
            service.TryFund(50).Should().Be(50);
        }

        [Test]
        public void Should_Debit_Transfer_Allowance_When_Levy_Balance_Is_Under_Allowance()
        {
            var service = new LevyBalanceService();
            service.Initialise(50, 50);
            service.TryFund(50).Should().Be(50);
            service.TryFundTransfer(50).Should().Be(0);
        }
    }
}
