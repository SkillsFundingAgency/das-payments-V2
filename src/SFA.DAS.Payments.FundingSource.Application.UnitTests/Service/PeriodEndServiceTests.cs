using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class PeriodEndServiceTests
    {
        private AutoMock mocker;
        private List<long> employerAccounts;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            employerAccounts = new List<long>();
            mocker.Mock<ILevyFundingSourceRepository>()
                .Setup(repo => repo.GetEmployerAccounts(It.IsAny<CancellationToken>()))
                .ReturnsAsync(employerAccounts);
        }

        [Test]
        public async Task Creates_Period_End_Command_For_Each_Employer()
        {
            var periodEndMessage = new PeriodEndRunningEvent
            {
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1920,
                    Period = 10
                },
                JobId = 1234
            };
            employerAccounts.Add(1234);
            employerAccounts.Add(5678);
            employerAccounts.Add(9012);
            var service = mocker.Create<PeriodEndService>();
            var commands = await service.GenerateEmployerPeriodEndCommands(periodEndMessage);
            commands.Count.Should().Be(3);
            commands.Any(command => command.AccountId == 1234).Should().BeTrue();
            commands.Any(command => command.AccountId == 5678).Should().BeTrue();
            commands.Any(command => command.AccountId == 9012).Should().BeTrue();
        }
    }
}