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
    public class ApprenticeshipPausedServiceTest
    {

        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }
        
        [Test]
        public async Task Updates_Paused_Apprenticeship()
        {
            var updatedApprenticeship = new UpdatedApprenticeshipPausedModel
            {
                ApprenticeshipId = 629959,
                PauseDate = DateTime.Today
            };

            var apprenticeshipModel = new ApprenticeshipModel
            {
                Id = updatedApprenticeship.ApprenticeshipId,
                StopDate = DateTime.Today.AddDays(-5),
                Status = ApprenticeshipStatus.Active,
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
                },
                Ukprn = 100,
                TransferSendingEmployerAccountId = 101,
                AccountId = 1,
                AgreementId = "1",
                IsLevyPayer = true,
                LegalEntityName = "Test Employer",
                FrameworkCode = 1,
                PathwayCode = 1,
            };

            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.Get(It.Is<long>(id => id == apprenticeshipModel.Id)))
                .ReturnsAsync(apprenticeshipModel);

            mocker.Mock<IApprenticeshipRepository>()
                .Setup(x => x.AddApprenticeshipPause(It.Is<ApprenticeshipPauseModel>(a => a.ApprenticeshipId == apprenticeshipModel.Id)))
                .Returns(Task.CompletedTask);

            var service = mocker.Create<ApprenticeshipPauseService>();
            await service.UpdateApprenticeship(updatedApprenticeship);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.UpdateApprenticeship(It.Is<ApprenticeshipModel>(model =>
                    model.Id == updatedApprenticeship.ApprenticeshipId
                    && model.Status == ApprenticeshipStatus.Paused
                    && model.ApprenticeshipPriceEpisodes.Count == 2
                    && model.ApprenticeshipPriceEpisodes[0].Cost == apprenticeshipModel.ApprenticeshipPriceEpisodes[0].Cost
                    && model.ApprenticeshipPriceEpisodes[0].StartDate == apprenticeshipModel.ApprenticeshipPriceEpisodes[0].StartDate
                    && model.ApprenticeshipPriceEpisodes[1].StartDate == apprenticeshipModel.ApprenticeshipPriceEpisodes[1].StartDate
                    && model.ApprenticeshipPriceEpisodes[1].Cost == apprenticeshipModel.ApprenticeshipPriceEpisodes[1].Cost
                    && model.Uln == apprenticeshipModel.Uln
                    && model.AgreedOnDate == apprenticeshipModel.AgreedOnDate
                    && model.EstimatedStartDate == apprenticeshipModel.EstimatedStartDate
                    && model.EstimatedEndDate == apprenticeshipModel.EstimatedEndDate
                    && model.ProgrammeType == apprenticeshipModel.ProgrammeType
                    && model.StandardCode == apprenticeshipModel.StandardCode
                    && model.Ukprn == apprenticeshipModel.Ukprn
                    && model.TransferSendingEmployerAccountId == apprenticeshipModel.TransferSendingEmployerAccountId
                    && model.AccountId == apprenticeshipModel.AccountId
                    && model.AgreementId == apprenticeshipModel.AgreementId
                    && model.IsLevyPayer == apprenticeshipModel.IsLevyPayer
                    && model.LegalEntityName == apprenticeshipModel.LegalEntityName
                    && model.FrameworkCode == apprenticeshipModel.FrameworkCode
                    && model.PathwayCode == apprenticeshipModel.FrameworkCode

                )), Times.Once);

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => x.Get(It.Is<long>(id => id == apprenticeshipModel.Id)), Times.Exactly(2));

            mocker.Mock<IApprenticeshipRepository>()
                .Verify(x => 
                    x.AddApprenticeshipPause(It.Is<ApprenticeshipPauseModel>(a => a.ApprenticeshipId == apprenticeshipModel.Id)),
                    Times.Once);
            
        }

    }
}
