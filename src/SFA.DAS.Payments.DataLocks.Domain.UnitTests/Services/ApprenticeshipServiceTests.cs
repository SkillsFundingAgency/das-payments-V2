using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Exceptions;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipServiceTests
    {
        private Autofac.Extras.Moq.AutoMock mocker;
        private ApprenticeshipModel apprenticeship;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.Add(It.IsAny<ApprenticeshipModel>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.GetDuplicates(It.IsAny<long>()))
                .ReturnsAsync(new List<ApprenticeshipDuplicateModel>
                    {new ApprenticeshipDuplicateModel {ApprenticeshipId = 321, Ukprn = 5678, Uln = 54321, Id = 1}});

            mocker.Mock<IApprenticeshipApprovedUpdatedService>()
                .Setup(x => x.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipApprovedModel>()))
                .ReturnsAsync(new ApprenticeshipModel());

            mocker.Mock<IApprenticeshipDataLockTriageService>()
                .Setup(x => x.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipDataLockTriageModel>()))
                .ReturnsAsync(new ApprenticeshipModel());

            apprenticeship = new ApprenticeshipModel
            {
                Id = 1234,
                AccountId = 12345,
                Ukprn = 123,
                Uln = 54321
            };
        }

        [Test]
        public async Task Stores_New_Apprenticeships()
        {
            var service = mocker.Create<ApprenticeshipService>();
            await service.NewApprenticeship(apprenticeship);
            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.Add(It.Is<ApprenticeshipModel>(model =>
                    model.Id == apprenticeship.Id
                    && model.AccountId == apprenticeship.AccountId
                    && model.Uln == apprenticeship.Uln
                    && model.Ukprn == apprenticeship.Ukprn)), Times.Once);
        }

        [Test]
        public void Throws_Exception_If_New_Apprenticeship_Already_Exists()
        {
            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.Get(It.Is<long>(id => id == apprenticeship.Id)))
                .ReturnsAsync(apprenticeship);
            var service = mocker.Create<ApprenticeshipService>();
            Assert.ThrowsAsync<ApprenticeshipAlreadyExistsException>(() => service.NewApprenticeship(apprenticeship));
        }

        [Test]
        public async Task Returns_Duplicates_Of_New_Apprenticeship()
        {
            var service = mocker.Create<ApprenticeshipService>();
            var duplicates = await service.NewApprenticeship(apprenticeship);
            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.GetDuplicates(It.Is<long>(uln => uln == apprenticeship.Uln)), Times.Once);
            duplicates.Count.Should().Be(1);
            duplicates.All(duplicate => duplicate.Id == 1).Should().BeTrue();
        }

        [Test]
        public async Task Stores_Duplicates_For_All_Providers()
        {
            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.GetDuplicates(It.IsAny<long>()))
                .ReturnsAsync(new List<ApprenticeshipDuplicateModel>
                {
                    new ApprenticeshipDuplicateModel {ApprenticeshipId = 321, Ukprn = 5678, Uln = 54321, Id = 1},
                    new ApprenticeshipDuplicateModel {ApprenticeshipId = 654, Ukprn = 5678, Uln = 54321, Id = 2},
                    new ApprenticeshipDuplicateModel {ApprenticeshipId = 321, Ukprn = 9012, Uln = 54321, Id = 3},
                    new ApprenticeshipDuplicateModel {ApprenticeshipId = 654, Ukprn = 9012, Uln = 54321, Id = 4},
                });

            var service = mocker.Create<ApprenticeshipService>();
            await service.NewApprenticeship(apprenticeship);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                        duplicates.Any(duplicate =>
                            duplicate.Ukprn == apprenticeship.Ukprn &&
                            duplicate.ApprenticeshipId == apprenticeship.Id))),
                    Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate =>
                        duplicate.Ukprn == 5678 && duplicate.ApprenticeshipId == apprenticeship.Id))), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate =>
                        duplicate.Ukprn == 9012 && duplicate.ApprenticeshipId == apprenticeship.Id))), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate =>
                        duplicate.Ukprn == apprenticeship.Ukprn && duplicate.ApprenticeshipId == 321))), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate =>
                        duplicate.Ukprn == apprenticeship.Ukprn && duplicate.ApprenticeshipId == 654))), Times.Once);
        }

        [Test]
        public async Task Update_Apprenticeship_Employer_IsLevyPayer_Flag_Correctly()
        {
            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.GetEmployerApprenticeships(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ApprenticeshipModel>
                {
                    new ApprenticeshipModel
                    {
                        IsLevyPayer = true
                    }
                });


            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.UpdateApprenticeships(It.IsAny<List<ApprenticeshipModel>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();


            var service = mocker.Create<ApprenticeshipService>();
          var expectedUpdatedApprenticeships =  await service.GetUpdatedApprenticeshipEmployerIsLevyPayerFlag(1);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.UpdateApprenticeships(It.Is<List<ApprenticeshipModel>>(o => o.Count == 1 && o[0].IsLevyPayer == false)),Times.Once);

            expectedUpdatedApprenticeships.Should().NotBeNull();
            expectedUpdatedApprenticeships.Should().HaveCount(1);
            expectedUpdatedApprenticeships[0].IsLevyPayer.Should().BeFalse();

        }

    }
}