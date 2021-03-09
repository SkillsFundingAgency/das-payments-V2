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
        private Mock<IPeriodEndRepository> repository;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            repository = mocker.Mock<IPeriodEndRepository>();
            sut = mocker.Create<ProviderRequiringReprocessingService>();
        }

        [Test]
        public async Task WhenUkprnExistsInTable_Then_AddToTableIsNotCalled()
        {
            repository.Setup(x => x.RecordForProviderThatRequiresReprocessing(-100))
                .ReturnsAsync(new ProviderRequiringReprocessingEntity());

            await sut.AddUkprnIfNotExists(-100);
      
            repository.Verify(x => x.AddProviderThatRequiredReprocessing(-100), Times.Never);
        }

        [Test]
        public async Task WhenUkrpnDoesNotExist_Then_AddToTableIsCalled()
        {
            await sut.AddUkprnIfNotExists(-100);

            repository.Verify(x => x.AddProviderThatRequiredReprocessing(-100), Times.Once);
        }
    }
}
