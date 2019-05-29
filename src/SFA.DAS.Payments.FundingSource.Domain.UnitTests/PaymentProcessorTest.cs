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
    public class PaymentProcessorTest
    {
        private Mock<ILevyPaymentProcessor> levyPaymentProcessorMock;
        private Mock<ICoInvestedPaymentProcessor> coInvestedPaymentProcessorMock;
        private IPaymentProcessor processor;
        private AutoMoqer mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = new AutoMoqer();
            levyPaymentProcessorMock = mocker.GetMock<ILevyPaymentProcessor>(MockBehavior.Strict);
            coInvestedPaymentProcessorMock = mocker.GetMock<ICoInvestedPaymentProcessor>(MockBehavior.Strict);
            processor = mocker.Resolve<PaymentProcessor>();
        }

        [TearDown]
        public void TearDown()
        {
            levyPaymentProcessorMock.Verify();
            coInvestedPaymentProcessorMock.Verify();
        }

        [Test]
        public void TestLevyOnlyCall()
        {
            // arrange
            var levyPayment = new LevyPayment { AmountDue = 100 };
            var requiredPayment = new RequiredPayment { AmountDue = 100 };

            levyPaymentProcessorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(new[] { levyPayment }).Verifiable();

            // act
            var actualPayments = processor.Process(requiredPayment);

            // assert
            actualPayments.Should().NotBeNull();
            actualPayments.Should().HaveCount(1);
            actualPayments[0].Should().BeSameAs(levyPayment);
        }

        [Test]
        public void TestLevyAndCoInvestedCall()
        {
            // arrange
            var levyPayment = new LevyPayment { AmountDue = 45 };
            var coInvestedPayment = new EmployerCoInvestedPayment { AmountDue = 55 };
            var requiredPayment = new RequiredPayment { AmountDue = 100 };

            levyPaymentProcessorMock.Setup(p => p.Process(requiredPayment)).Returns(new[] { levyPayment }).Verifiable();
            coInvestedPaymentProcessorMock.Setup(p => p.Process(It.Is<RequiredPayment>(rp => rp.AmountDue == 55))).Returns(new[] { coInvestedPayment }).Verifiable();

            // act
            var actualPayments = processor.Process(requiredPayment);

            // assert
            actualPayments.Should().NotBeNull();
            actualPayments.Should().HaveCount(2);
            actualPayments[0].Should().BeSameAs(levyPayment);
            actualPayments[1].Should().BeSameAs(coInvestedPayment);
        }

        [Test]
        public void Should_Use_TransferProcessor_For_Transfer_Payments()
        {
            var requiredPayment = new RequiredPayment { AmountDue = 50, IsTransfer = true, SfaContributionPercentage = .95M };
            mocker.GetMock<ITransferPaymentProcessor>()
                .Setup(x => x.Process(requiredPayment))
                .Returns(new[] { new TransferPayment { AmountDue = 50, Type = FundingSourceType.Transfer } });
            var paymentProcessor = mocker.Resolve<PaymentProcessor>();
            var payments = paymentProcessor.Process(requiredPayment);
            payments.Count.Should().Be(1);
            payments.All(p => p.Type == FundingSourceType.Transfer && p is TransferPayment).Should().BeTrue();
            mocker.GetMock<ILevyPaymentProcessor>()
                .Verify(x => x.Process(It.IsAny<RequiredPayment>()), Times.Never);
        }

        [Test]
        public void Should_Return_Unable_To_Pay_Transfer_If_No_Transfer_Allowance()
        {
            var requiredPayment = new RequiredPayment { AmountDue = 50, IsTransfer = true, SfaContributionPercentage = .95M };
            mocker.GetMock<ITransferPaymentProcessor>()
                .Setup(x => x.Process(requiredPayment))
                .Returns(new FundingSourcePayment[0]);
            var paymentProcessor = mocker.Resolve<PaymentProcessor>();
            var payments = paymentProcessor.Process(requiredPayment);
            payments.Count.Should().Be(1);
            payments.All(p => p.Type == FundingSourceType.Transfer && p is UnableToFundTransferPayment && p.AmountDue == 50).Should().BeTrue();
        }

        [Test]
        public void Should_Return_Transfer_And_Unable_To_Pay_Transfer_If_Not_Enough_Allowance_To_Cover_Whole_Transfer_Amount()
        {
            var requiredPayment = new RequiredPayment { AmountDue = 50, IsTransfer = true, SfaContributionPercentage = .95M };
            mocker.GetMock<ITransferPaymentProcessor>()
                .Setup(x => x.Process(requiredPayment))
                .Returns(new[] { new TransferPayment { AmountDue = 40, Type = FundingSourceType.Transfer } });
            var paymentProcessor = mocker.Resolve<PaymentProcessor>();
            var payments = paymentProcessor.Process(requiredPayment);
            payments.Count(p => p.Type == FundingSourceType.Transfer && p is TransferPayment && p.AmountDue == 40).Should().Be(1);
            payments.Count(p => p.Type == FundingSourceType.Transfer && p is UnableToFundTransferPayment && p.AmountDue == 10).Should().Be(1);
        }


        [Test]
        public void Should_Return_Refund_Transfer_Payment()
        {
            var requiredPayment = new RequiredPayment { AmountDue = -50, IsTransfer = true, SfaContributionPercentage = .95M };
            mocker.GetMock<ITransferPaymentProcessor>()
                .Setup(x => x.Process(requiredPayment))
                .Returns(new[] { new TransferPayment { AmountDue = -50, Type = FundingSourceType.Transfer } });
            var paymentProcessor = mocker.Resolve<PaymentProcessor>();
            var payments = paymentProcessor.Process(requiredPayment);
            payments.Count.Should().Be(1);
            payments.Count(p => p.Type == FundingSourceType.Transfer && p is TransferPayment && p.AmountDue == -50).Should().Be(1);
        }

    }
}