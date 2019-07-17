using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    public class ApprenticeshipDataLockTriageServiceTest
    {

        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

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
                FrameworkCode = default(int?),
                PathwayCode = default(int?),
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
                },
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
                FrameworkCode = 0,
                PathwayCode = 0,

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
                },
                Ukprn = 100,
                Status = ApprenticeshipStatus.Active,
                StopDate = DateTime.Today,
                TransferSendingEmployerAccountId = 101,
                AccountId = 1,
                AgreementId = "1",
                IsLevyPayer = true,
                LegalEntityName = "Test Employer",
            };

            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.Get(apprenticeshipModel.Id))
                .ReturnsAsync(apprenticeshipModel);

            var service = mocker.Create<ApprenticeshipDataLockTriageService>();
            await service.UpdateApprenticeship(updatedApprenticeship);

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
                    && model.ApprenticeshipPriceEpisodes[2].Cost == updatedApprenticeship.ApprenticeshipPriceEpisodes[1].Cost
                    && model.Ukprn == apprenticeshipModel.Ukprn 
                    && model.Status == apprenticeshipModel.Status
                    && model.StopDate == apprenticeshipModel.StopDate
                    && model.TransferSendingEmployerAccountId == apprenticeshipModel.TransferSendingEmployerAccountId
                    && model.AccountId == apprenticeshipModel.AccountId
                    && model.AgreementId == apprenticeshipModel.AgreementId
                    && model.IsLevyPayer == apprenticeshipModel.IsLevyPayer
                    && model.LegalEntityName == apprenticeshipModel.LegalEntityName
                    )), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.Get(It.Is<long>(id => id == apprenticeshipModel.Id)), Times.Exactly(2));

        }

    }
}
