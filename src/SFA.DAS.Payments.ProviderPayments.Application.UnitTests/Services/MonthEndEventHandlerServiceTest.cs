using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

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


            monthEndEventHandlerService = new MonthEndEventHandlerService(providerPaymentsRepository.Object);
        }


    }
}
