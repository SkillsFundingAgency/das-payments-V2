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

        [Test]
        public void CalculateFundingAccountId_ShouldReturn_AccountId_IfFailedTransfer()
        {
            var accountId = 112;
            new CalculatedRequiredLevyAmount
                {
                    AccountId = accountId,
                    TransferSenderAccountId = 114
                }
                .CalculateFundingAccountId(true)
                .Should()
                .Be(accountId);
        }

        [Test]
        public void CalculateFundingAccountId_ShouldReturn_TransferSenderAccountId_IfTransfer()
        {
            var transferSenderAccountId = 114;
            new CalculatedRequiredLevyAmount
                {
                    AccountId = 112,
                    TransferSenderAccountId = transferSenderAccountId
            }
                .CalculateFundingAccountId(false)
                .Should()
                .Be(transferSenderAccountId);
        }

        [Test]
        public void CalculateFundingAccountId_ShouldReturn_AccountId_IfNotTransfer()
        {
            var accountId = 112;
            new CalculatedRequiredLevyAmount
                {
                    AccountId = accountId,
                    TransferSenderAccountId = null
                }
                .CalculateFundingAccountId(false)
                .Should()
                .Be(accountId);
        }
    }
}