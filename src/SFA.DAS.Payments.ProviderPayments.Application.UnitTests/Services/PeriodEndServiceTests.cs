using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class PeriodEndServiceTests
    {
        private AutoMock mocker;
        private List<long> providers;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            providers = new List<long>();
            mocker.Mock<IProviderPaymentsRepository>()
                .Setup(repo => repo.GetMonthEndProviders(It.IsAny<CollectionPeriod>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(providers);
        }

        [Test]
        public async Task Generates_Provider_Period_End_Command_For_All_Providers()
        {
            providers.Add(1234);
            providers.Add(5678);
            providers.Add(9012);
            var periodEndStopped = new PeriodEndStoppedEvent
            {
                CollectionPeriod = new CollectionPeriod {  AcademicYear = 1920, Period = 10},
                JobId = 123
            };
            var service = mocker.Create<PeriodEndService>();
            var commands = await service.GenerateProviderMonthEndCommands(periodEndStopped);
            commands.Count.Should().Be(3);
            commands.Any(command => command.Ukprn == 1234).Should().BeTrue();
            commands.Any(command => command.Ukprn == 5678).Should().BeTrue();
            commands.Any(command => command.Ukprn == 9012).Should().BeTrue();
        }

        [Test]
        public async Task Commands_Are_Created_Using_Dc_JobId()
        {
            providers.Add(1234);
            var periodEndStopped = new PeriodEndStoppedEvent
            {
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 10 },
                JobId = 123
            };
            var service = mocker.Create<PeriodEndService>();
            var commands = await service.GenerateProviderMonthEndCommands(periodEndStopped);
            commands.Count.Should().Be(1);
            commands.Any(command => command.JobId == 123).Should().BeTrue();
        }
    }
}