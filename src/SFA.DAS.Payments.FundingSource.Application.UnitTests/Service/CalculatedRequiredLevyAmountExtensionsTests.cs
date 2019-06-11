using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class CalculatedRequiredLevyAmountExtensionsTests
    {
        [Test]
        public void Should_Return_False_If_SenderAccountId_Is_Null()
        {
            new CalculatedRequiredLevyAmount
                {
                    AccountId = 1,
                    TransferSenderAccountId = null,
                }
                .IsTransfer()
                .Should()
                .BeFalse();
        }

        [Test]
        public void Should_Return_False_If_SenderAccountId_Is_Same_As_AccountId()
        {
            new CalculatedRequiredLevyAmount
                {
                    AccountId = 1,
                    TransferSenderAccountId = 1,
                }
                .IsTransfer()
                .Should()
                .BeFalse();
        }

        [Test]
        public void Should_Return_True_If_SenderAccountId_Is_Not_Null_And_Different_To_AccountId()
        {
            new CalculatedRequiredLevyAmount
                {
                    AccountId = 1,
                    TransferSenderAccountId = 2,
                }
                .IsTransfer()
                .Should()
                .BeTrue();
        }
    }
}