using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipUpdatedProcessorTests
    {
        private AutoMock mocker;
        private ApprenticeshipUpdated updatedApprenticeship;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IMapper>()
                .Setup(x => x.Map<ApprenticeshipModel>(It.IsAny<ApprenticeshipUpdated>()))
                .Returns<ApprenticeshipUpdated>(ev => new ApprenticeshipModel
                {
                    AccountId = ev.EmployerAccountId,
                    Ukprn = ev.Ukprn,
                    Id = ev.Id,
                    Uln = ev.Uln
                });

            mocker.Mock<IActorDataCache<List<ApprenticeshipModel>>>()
                .Setup(x => x.AddOrReplace(It.IsAny<string>(), It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            updatedApprenticeship = new ApprenticeshipUpdated
            {
                Id = 123,
                Ukprn = 123456,
                Uln = 54321,
                EmployerAccountId = 1234
            };
        }

        [Test]
        public async Task Stores_New_Apprentices_In_The_Cache()
        {
            mocker.Mock<IActorDataCache<List<ApprenticeshipModel>>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<List<ApprenticeshipModel>>(false, null));
            var processor = mocker.Create<ApprenticeshipUpdatedProcessor>();
            await processor.ProcessApprenticeshipUpdate(updatedApprenticeship);

            mocker.Mock<IActorDataCache<List<ApprenticeshipModel>>>()
                .Verify(x => x.TryGet(It.Is<string>(key => key == updatedApprenticeship.Uln.ToString()),
                    It.IsAny<CancellationToken>()), Times.Once);

            mocker.Mock<IActorDataCache<List<ApprenticeshipModel>>>()
                .Verify(x => x.AddOrReplace(It.Is<string>(key => key == updatedApprenticeship.Uln.ToString()),
                    It.Is<List<ApprenticeshipModel>>(list => list.Count == 1 && list.First().Id == updatedApprenticeship.Id),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}