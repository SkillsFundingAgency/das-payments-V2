using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;
using SFA.DAS.Payments.PeriodEnd.Application.Services;
using SFA.DAS.Payments.PeriodEnd.Model;

namespace SFA.DAS.Payments.PeriodEnd.Application.UnitTests.ProviderRequiringReprocessingServiceTests
{
    [TestFixture]
    public class AddUkprnIfNotExistsTests
    {
        private AutoMock mocker;
        private ProviderRequiringReprocessingService sut;
        private Mock<IProvidersRequiringReprocessingRepository> repository;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            repository = mocker.Mock<IProvidersRequiringReprocessingRepository>();
            sut = mocker.Create<ProviderRequiringReprocessingService>();
        }
    }
}
