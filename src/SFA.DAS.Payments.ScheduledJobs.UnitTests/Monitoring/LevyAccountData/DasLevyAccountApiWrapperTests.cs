using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests.Monitoring.LevyAccountData
{
    [TestFixture]
    public class DasLevyAccountApiWrapperTests
    {
        private DasLevyAccountApiWrapper sut;
        private Mock<IAccountApiClient> accountApiClient;

        private readonly PagedApiResponseViewModel<AccountWithBalanceViewModel> apiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
        {
            TotalPages = 1,
            Data = new List<AccountWithBalanceViewModel>
            {
                new AccountWithBalanceViewModel
                {
                    AccountId = 1,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    AccountName = "Test Ltd",
                    IsLevyPayer = true
                }
            }
        };

        [SetUp]
        public void SetUp()
        {
            var mocker = AutoMock.GetLoose();
            mocker.Mock<IPaymentLogger>();
            accountApiClient = mocker.Mock<IAccountApiClient>();
            sut = mocker.Create<DasLevyAccountApiWrapper>(new NamedParameter("accountApiBatchSize", 1000));
        }

        [Test]
        public async Task GetDasLevyAccountDetails_Should_Call_GetPageOfAccounts_To_Get_LevyAccounts()
        {
            accountApiClient
                .Setup(x => x.GetPageOfAccounts(1, It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(apiResponseViewModel);

            var dasLevyAccountDetails = await sut.GetDasLevyAccountDetails();

            accountApiClient
                .Verify(x => x.GetPageOfAccounts(1, It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Exactly(2));

            dasLevyAccountDetails.Should().NotBeNullOrEmpty();
            dasLevyAccountDetails.Count.Should().Be(1);
            var levyAccountModel = dasLevyAccountDetails.ElementAt(0);
            levyAccountModel.AccountId.Should().Be(1);
            levyAccountModel.Balance.Should().Be(100m);
            levyAccountModel.TransferAllowance.Should().Be(10m);
            levyAccountModel.IsLevyPayer.Should().Be(true);
        }

        [Test]
        public async Task When_GetTotalPageSize_Throws_Error_GetDasLevyAccountDetails_Should_Return_EmptyList()
        {
            accountApiClient
                .Setup(x => x.GetPageOfAccounts(1, It.IsAny<int>(), It.IsAny<DateTime?>()))
                .Throws<Exception>();

            var dasLevyAccountDetails = await sut.GetDasLevyAccountDetails();

            accountApiClient
                .Verify(x => x.GetPageOfAccounts(1, It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Exactly(1));

            dasLevyAccountDetails.Should().NotBeNull();
            dasLevyAccountDetails.Count.Should().Be(0);
        }

        [Test]
        public async Task When_GetPageOfLevyAccounts_Throws_Error_GetDasLevyAccountDetails_Should_Return_EmptyList()
        {
            accountApiClient
                .SetupSequence(x => x.GetPageOfAccounts(1, It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(apiResponseViewModel)
                .Throws<Exception>();

            var dasLevyAccountDetails = await sut.GetDasLevyAccountDetails();

            accountApiClient
                .Verify(x => x.GetPageOfAccounts(1, It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Exactly(2));

            dasLevyAccountDetails.Should().NotBeNull();
            dasLevyAccountDetails.Count.Should().Be(0);
        }
    }
}
