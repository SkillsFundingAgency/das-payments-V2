using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Model.Enum;
using SFA.DAS.Payments.Model.Core.Entities;
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
            var payments = new List<PaymentDataEntity>
            {
                new PaymentDataEntity()
                {
                    Ukprn = 1000,
                    FundingSource = (int)FundingSourceType.CoInvestedEmployer
                }
            };

            providerPaymentsRepository = new Mock<IProviderPaymentsRepository>();

            providerPaymentsRepository
                .Setup(o => o.GetMonthEndPaymentsAsync(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments)
                .Verifiable();

            monthEndEventHandlerService = new MonthEndEventHandlerService(providerPaymentsRepository.Object);
        }


        [Test]
        public async Task ShouldCallRepositoryAsync()
        {
            short year = 2018;
            byte month = 9;
            long ukprn = 1000;
            var cancellationToken = new CancellationToken();

            var result = await monthEndEventHandlerService.GetMonthEndPaymentsAsync(year, month, ukprn, cancellationToken);

            providerPaymentsRepository.Verify();
        }

    }
}
