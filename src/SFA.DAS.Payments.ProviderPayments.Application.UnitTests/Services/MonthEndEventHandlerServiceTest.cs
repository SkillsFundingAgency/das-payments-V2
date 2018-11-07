using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class MonthEndEventHandlerServiceTest
    {
        private MonthEndEventHandlerService monthEndEventHandlerService;
        private Mock<IProviderPaymentsRepository> providerPaymentsRepository;

        [SetUp]
        public void Setup()
        {
            providerPaymentsRepository = new Mock<IProviderPaymentsRepository>();
            providerPaymentsRepository
                .Setup(o => o.GetMonthEndUkprns(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<long> { 1, 2, 3 })
                .Verifiable();

            monthEndEventHandlerService = new MonthEndEventHandlerService(providerPaymentsRepository.Object);
        }

        [Test]
        public async Task GetMonthEndUkprnsShouldCallRequiredServices()
        {
            await monthEndEventHandlerService.GetMonthEndUkprns(2018, 3);
            providerPaymentsRepository.Verify();
        }

    }
}
