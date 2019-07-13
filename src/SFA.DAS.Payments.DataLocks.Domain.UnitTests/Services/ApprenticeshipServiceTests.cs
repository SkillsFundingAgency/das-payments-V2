using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.Model.Core.Entities;

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
                .ReturnsAsync(new List<ApprenticeshipDuplicateModel> { new ApprenticeshipDuplicateModel { ApprenticeshipId = 321, Ukprn = 5678, Uln = 54321, Id = 1 } });
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
            Assert.ThrowsAsync<InvalidOperationException>(() => service.NewApprenticeship(apprenticeship));
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
                    new ApprenticeshipDuplicateModel { ApprenticeshipId = 321, Ukprn = 5678, Uln = 54321, Id = 1 },
                    new ApprenticeshipDuplicateModel { ApprenticeshipId = 654, Ukprn = 5678, Uln = 54321, Id = 2 },
                    new ApprenticeshipDuplicateModel { ApprenticeshipId = 321, Ukprn = 9012, Uln = 54321, Id = 3 },
                    new ApprenticeshipDuplicateModel { ApprenticeshipId = 654, Ukprn = 9012, Uln = 54321, Id = 4 },
                });

            var service = mocker.Create<ApprenticeshipService>();
            await service.NewApprenticeship(apprenticeship);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate => duplicate.Ukprn == apprenticeship.Ukprn && duplicate.ApprenticeshipId == apprenticeship.Id))), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate => duplicate.Ukprn == 5678 && duplicate.ApprenticeshipId == apprenticeship.Id))), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate => duplicate.Ukprn == 9012 && duplicate.ApprenticeshipId == apprenticeship.Id))), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate => duplicate.Ukprn == apprenticeship.Ukprn && duplicate.ApprenticeshipId == 321))), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.StoreDuplicates(It.Is<List<ApprenticeshipDuplicateModel>>(duplicates =>
                    duplicates.Any(duplicate => duplicate.Ukprn == apprenticeship.Ukprn && duplicate.ApprenticeshipId == 654))), Times.Once);
        }

        [Test]
        public async Task Updates_Apprenticeship()
        {
            var updatedApprenticeship = new UpdatedApprenticeshipApprovedModel
            {
                ApprenticeshipId = 629959,
                Uln = 123456,
                AgreedOnDate = DateTime.Today.AddDays(-1),
                EstimatedStartDate = DateTime.Today,
                EstimatedEndDate = DateTime.Today.AddYears(1),
                ProgrammeType = 26,
                StandardCode = 18,

                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = new DateTime(2017,08,06),
                        EndDate = new DateTime(2017,08,30),
                        Cost = 15000.00m
                    },
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = new DateTime(2017,09,06),
                        Cost = 20000.00m
                    }
                }

            };

            var apprenticeshipModel = new ApprenticeshipModel
            {
                Id = updatedApprenticeship.ApprenticeshipId,
                Uln = updatedApprenticeship.Uln,
                AgreedOnDate = DateTime.Today.AddDays(-2),
                EstimatedStartDate = DateTime.Today.AddDays(-1),
                EstimatedEndDate = DateTime.Today.AddYears(2),
                ProgrammeType = 25,
                StandardCode = 17,

                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 1,
                        ApprenticeshipId = 629959,
                        StartDate = new DateTime(2017,08,06),
                        Cost = 15000.00m
                    },
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id =161,
                        ApprenticeshipId = 629959,
                        StartDate = new DateTime(2017,08,10),
                        Cost = 1000.00m
                    }
                }
            };


            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.Get(It.Is<long>(id => id == apprenticeshipModel.Id)))
                .ReturnsAsync(apprenticeshipModel);

            var service = mocker.Create<ApprenticeshipService>();
            await service.UpdateApprenticeship(updatedApprenticeship);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.UpdateApprenticeship(It.Is<ApprenticeshipModel>(model =>
                    model.Id == updatedApprenticeship.ApprenticeshipId
                    && model.AgreedOnDate == updatedApprenticeship.AgreedOnDate
                    && model.Uln == updatedApprenticeship.Uln
                    && model.EstimatedStartDate == updatedApprenticeship.EstimatedStartDate
                    && model.EstimatedEndDate == updatedApprenticeship.EstimatedEndDate
                    && model.StandardCode == updatedApprenticeship.StandardCode
                    && model.ProgrammeType == updatedApprenticeship.ProgrammeType
                    && model.FrameworkCode == updatedApprenticeship.FrameworkCode
                    && model.PathwayCode == updatedApprenticeship.PathwayCode
                    
                    && model.ApprenticeshipPriceEpisodes.Count == 3
                    && model.ApprenticeshipPriceEpisodes[0].EndDate == updatedApprenticeship.ApprenticeshipPriceEpisodes[0].EndDate
                    && model.ApprenticeshipPriceEpisodes[1].Removed == true
                    && model.ApprenticeshipPriceEpisodes[2].StartDate == updatedApprenticeship.ApprenticeshipPriceEpisodes[1].StartDate
                    && model.ApprenticeshipPriceEpisodes[2].Cost == updatedApprenticeship.ApprenticeshipPriceEpisodes[1].Cost)
                
                ), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.Get(It.Is<long>(id => id == apprenticeshipModel.Id)),Times.Exactly(2));

        }

        [Test]
        public async Task Updates_Apprenticeship_DataLockTriage()
        {
            var updatedApprenticeship = new UpdatedApprenticeshipDataLockTriageModel
            {
                ApprenticeshipId = 629959,
                AgreedOnDate = DateTime.Today.AddDays(-1),
                ProgrammeType = 26,
                StandardCode = 18,

                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = new DateTime(2017,08,06),
                        EndDate = new DateTime(2017,08,30),
                        Cost = 15000.00m
                    },
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = new DateTime(2017,09,06),
                        Cost = 20000.00m
                    }
                }
            };

            var apprenticeshipModel = new ApprenticeshipModel
            {
                Id = updatedApprenticeship.ApprenticeshipId,
                Uln = 1,
                AgreedOnDate = DateTime.Today.AddDays(-2),
                EstimatedStartDate = DateTime.Today.AddDays(-1),
                EstimatedEndDate = DateTime.Today.AddYears(2),
                ProgrammeType = 25,
                StandardCode = 17,

                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 1,
                        ApprenticeshipId = updatedApprenticeship.ApprenticeshipId,
                        StartDate = new DateTime(2017,08,06),
                        Cost = 15000.00m
                    },
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id =161,
                        ApprenticeshipId = updatedApprenticeship.ApprenticeshipId,
                        StartDate = new DateTime(2017,08,10),
                        Cost = 1000.00m
                    }
                }
            };
            
            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.Get(It.Is<long>(id => id == apprenticeshipModel.Id)))
                .ReturnsAsync(apprenticeshipModel);

            var service = mocker.Create<ApprenticeshipService>();
            await service.UpdateApprenticeshipForDataLockTriage(updatedApprenticeship);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.UpdateApprenticeship(It.Is<ApprenticeshipModel>(model =>
                    model.Id == updatedApprenticeship.ApprenticeshipId
                    && model.AgreedOnDate == updatedApprenticeship.AgreedOnDate
                    && model.StandardCode == updatedApprenticeship.StandardCode
                    && model.ProgrammeType == updatedApprenticeship.ProgrammeType
                    && model.FrameworkCode == updatedApprenticeship.FrameworkCode
                    && model.PathwayCode == updatedApprenticeship.PathwayCode
                    && model.ApprenticeshipPriceEpisodes.Count == 3
                    && model.ApprenticeshipPriceEpisodes[0].EndDate == updatedApprenticeship.ApprenticeshipPriceEpisodes[0].EndDate
                    && model.ApprenticeshipPriceEpisodes[1].Removed == true
                    && model.ApprenticeshipPriceEpisodes[2].StartDate == updatedApprenticeship.ApprenticeshipPriceEpisodes[1].StartDate
                    && model.ApprenticeshipPriceEpisodes[2].Cost == updatedApprenticeship.ApprenticeshipPriceEpisodes[1].Cost)
                ), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.Get(It.Is<long>(id => id == apprenticeshipModel.Id)), Times.Exactly(2));

        }
    }
}