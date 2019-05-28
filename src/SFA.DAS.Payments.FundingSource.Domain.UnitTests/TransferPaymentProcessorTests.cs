using System.Linq;
using AutoMoqCore;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class TransferPaymentProcessorTests
    {
        private AutoMoqer mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = new AutoMoqer();
        }

        [Test]
        public void Should_Return_Transfer_Payment()
        {
            mocker.GetMock<ILevyBalanceService>()
                .Setup(x => x.TryFundTransfer(It.IsAny<decimal>()))
                .Returns(50);

            var processor = mocker.Resolve<TransferPaymentProcessor>();
            var payments = processor.Process(new RequiredPayment
            {
                AmountDue = 50,
                SfaContributionPercentage = .95M
            });
            payments.Count.Should().Be(1);
            payments.All(p => p.AmountDue == 50 && p.Type == FundingSourceType.Transfer).Should().BeTrue();
        }

        [Test]
        public void Should_Create_Transfer_Payment_If_Enough_Allowance()
        {
            mocker.GetMock<ILevyBalanceService>()
                .Setup(x => x.TryFundTransfer(It.IsAny<decimal>()))
                .Returns(50);

            var processor = mocker.Resolve<TransferPaymentProcessor>();
            var payments = processor.Process(new RequiredPayment
            {
                AmountDue = 50,
                SfaContributionPercentage = .95M
            });
            payments.Count.Should().Be(1);
            payments.All(p => p.AmountDue == 50 && p.Type == FundingSourceType.Transfer).Should().BeTrue();
        }

        [Test]
        public void Should_Create_Partial_Transfer_Payment_If_Not_Enough_Allowance()
        {
            mocker.GetMock<ILevyBalanceService>()
                .Setup(x => x.TryFundTransfer(It.IsAny<decimal>()))
                .Returns(25);

            var processor = mocker.Resolve<TransferPaymentProcessor>();
            var payments = processor.Process(new RequiredPayment
            {
                AmountDue = 50,
                SfaContributionPercentage = .95M
            });
            payments.Count.Should().Be(1);
            payments.All(p =>  p is TransferPayment).Should().BeTrue();
        }

        [Test]
        public void Should_Not_Create_A_Payment_If_No_Transfer_Allowance_Remaining()
        {
            mocker.GetMock<ILevyBalanceService>()
                .Setup(x => x.TryFundTransfer(It.IsAny<decimal>()))
                .Returns(0);

            var processor = mocker.Resolve<TransferPaymentProcessor>();
            var payments = processor.Process(new RequiredPayment
            {
                AmountDue = 50,
                SfaContributionPercentage = .95M
            });
            payments.Any().Should().BeFalse();
        }
    }
}