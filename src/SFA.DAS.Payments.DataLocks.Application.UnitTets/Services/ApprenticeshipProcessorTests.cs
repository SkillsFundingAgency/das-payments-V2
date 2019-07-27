using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipProcessorTests
    {
        private Autofac.Extras.Moq.AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IMapper>()
                .Setup(x => x.Map<ApprenticeshipModel>(It.IsAny<ApprenticeshipCreatedEvent>()))
                .Returns<ApprenticeshipCreatedEvent>(ev => new ApprenticeshipModel
                {
                    AccountId = ev.AccountId,
                    Ukprn = ev.ProviderId,
                    Id = ev.ApprenticeshipId,
                    Uln = long.Parse(ev.Uln)
                });
            mocker.Mock<IMapper>()
                .Setup(x => x.Map<ApprenticeshipUpdated>(It.IsAny<ApprenticeshipModel>()))
                .Returns<ApprenticeshipModel>(model => new ApprenticeshipUpdated
                {
                    EmployerAccountId = model.AccountId,
                    Ukprn = model.Ukprn,
                    Id = model.Id,
                    Uln = model.Uln
                });

            mocker.Mock<IEndpointInstance>()
                .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IEndpointInstanceFactory>()
                .Setup(x => x.GetEndpointInstance())
                .ReturnsAsync(mocker.Mock<IEndpointInstance>().Object);
        }

        [Test]
        public async Task Processes_ApprenticeshipCreated()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                StartDate = DateTime.Today,
                AccountLegalEntityPublicHashedId = "1234567890",
                AgreedOn = DateTime.Today.AddDays(-1),
                ApprenticeshipId = 12,
                CreatedOn = DateTime.Today.AddDays(-2),
                EndDate = DateTime.Today.AddYears(1),
                LegalEntityName = "Test Employer",
                ProviderId = 1234,
                TrainingCode = "52",
                TrainingType = ProgrammeType.Standard,
                TransferSenderId = 123456,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[] { new PriceEpisode { FromDate = DateTime.Today, Cost = 1000M } }
            };
            mocker.Mock<IApprenticeshipService>()
                .Setup(svc => svc.NewApprenticeship(It.IsAny<ApprenticeshipModel>()))
                .ReturnsAsync(() => new List<ApprenticeshipDuplicateModel>());

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.Process(approvalsEvent);

            mocker.Mock<IApprenticeshipService>()
                .Verify(svc => svc.NewApprenticeship(It.Is<ApprenticeshipModel>(model =>
                    model.AccountId == approvalsEvent.AccountId
                    && model.Id == approvalsEvent.ApprenticeshipId
                    && model.Ukprn == approvalsEvent.ProviderId
                    && model.Uln.ToString() == approvalsEvent.Uln)
                ), Times.Once);

        }

        [Test]
        public async Task Processing_Of_ApprenticeshipCreated_Publishes_ApprenticeshipUpdated_Event()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                StartDate = DateTime.Today,
                AccountLegalEntityPublicHashedId = "1234567890",
                AgreedOn = DateTime.Today.AddDays(-1),
                ApprenticeshipId = 12,
                CreatedOn = DateTime.Today.AddDays(-2),
                EndDate = DateTime.Today.AddYears(1),
                LegalEntityName = "Test Employer",
                ProviderId = 1234,
                TrainingCode = "52",
                TrainingType = ProgrammeType.Standard,
                TransferSenderId = 123456,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[] { new PriceEpisode { FromDate = DateTime.Today, Cost = 1000M } }
            };
            mocker.Mock<IApprenticeshipService>()
                .Setup(svc => svc.NewApprenticeship(It.IsAny<ApprenticeshipModel>()))
                .ReturnsAsync(() => new List<ApprenticeshipDuplicateModel>());

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.Process(approvalsEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev =>
                        ev.EmployerAccountId == approvalsEvent.AccountId
                        && ev.Id == approvalsEvent.ApprenticeshipId
                        && ev.Ukprn == approvalsEvent.ProviderId
                        && ev.Uln.ToString() == approvalsEvent.Uln),
                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task ApprenticeshipCreated_Triggers_ApprenticeshipUpdated_Event_With_Duplicates()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                StartDate = DateTime.Today,
                AccountLegalEntityPublicHashedId = "1234567890",
                AgreedOn = DateTime.Today.AddDays(-1),
                ApprenticeshipId = 12,
                CreatedOn = DateTime.Today.AddDays(-2),
                EndDate = DateTime.Today.AddYears(1),
                LegalEntityName = "Test Employer",
                ProviderId = 1234,
                TrainingCode = "52",
                TrainingType = ProgrammeType.Standard,
                TransferSenderId = 123456,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[] { new PriceEpisode { FromDate = DateTime.Today, Cost = 1000M } }
            };
            mocker.Mock<IApprenticeshipService>()
                .Setup(svc => svc.NewApprenticeship(It.IsAny<ApprenticeshipModel>()))
                .ReturnsAsync(() => new List<ApprenticeshipDuplicateModel>
                {
                    new ApprenticeshipDuplicateModel
                    {
                        Id = 1,
                        Ukprn = 4321,
                        Uln = 123456,
                        ApprenticeshipId = 13
                    }
                });

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.Process(approvalsEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev =>
                        ev.EmployerAccountId == approvalsEvent.AccountId
                        && ev.Id == approvalsEvent.ApprenticeshipId
                        && ev.Ukprn == approvalsEvent.ProviderId
                        && ev.Uln.ToString() == approvalsEvent.Uln
                        && ev.Duplicates.Count == 1
                        && ev.Duplicates.All(duplicate => duplicate.ApprenticeshipId == 13 && duplicate.Ukprn == 4321)),

                    It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task Processes_UpdatedApprenticeship()
        {
            var approvalsEvent = new ApprenticeshipUpdatedApprovedEvent()
            {
                Uln = "12345",
                ApprenticeshipId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                ApprovedOn = DateTime.Today,
                TrainingCode = "ABC",
                TrainingType = ProgrammeType.Standard,
                PriceEpisodes = new PriceEpisode[1]
                {
                    new PriceEpisode
                    {
                        FromDate = DateTime.Today,
                        ToDate = DateTime.Today.AddYears(1),
                        Cost = 1000m
                    }
                }
            };

            mocker.Mock<IApprenticeshipApprovedUpdatedService>()
                .Setup(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipApprovedModel>()))
                .ReturnsAsync(() => new ApprenticeshipModel
                {
                    Id = approvalsEvent.ApprenticeshipId,
                    Uln = long.Parse(approvalsEvent.Uln),

                });

            mocker.Mock<IMapper>()
                .Setup(x => x.Map<UpdatedApprenticeshipApprovedModel>(It.IsAny<ApprenticeshipUpdatedApprovedEvent>()))
                .Returns<ApprenticeshipUpdatedApprovedEvent>(model => new UpdatedApprenticeshipApprovedModel
                {
                    ApprenticeshipId = model.ApprenticeshipId,
                    Uln = long.Parse(model.Uln),
                    EstimatedStartDate = model.StartDate,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>()
                });

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessUpdatedApprenticeship(approvalsEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev =>
                        ev.Id == approvalsEvent.ApprenticeshipId
                        && ev.Uln.ToString() == approvalsEvent.Uln),
                    It.IsAny<PublishOptions>()), Times.Once);

            mocker.Mock<IMapper>()
                .Verify(x => x.Map<UpdatedApprenticeshipApprovedModel>(It.IsAny<ApprenticeshipUpdatedApprovedEvent>()), Times.Once);

        }


        [Test]
        public async Task Process_Apprenticeship_DataLock_Triage()
        {
            var dataLockTriageApprovedEvent = new DataLockTriageApprovedEvent()
            {
                ApprenticeshipId = 1,
                ApprovedOn = DateTime.Today,
                TrainingCode = "ABC",
                TrainingType = ProgrammeType.Standard,
                PriceEpisodes = new PriceEpisode[1]
                {
                    new PriceEpisode
                    {
                        FromDate = DateTime.Today,
                        ToDate = DateTime.Today.AddYears(1),
                        Cost = 1000m
                    }
                }
            };

            mocker.Mock<IMapper>()
                .Setup(x => x.Map<UpdatedApprenticeshipDataLockTriageModel>(It.IsAny<DataLockTriageApprovedEvent>()))
                .Returns<DataLockTriageApprovedEvent>(model => new UpdatedApprenticeshipDataLockTriageModel
                {
                    ApprenticeshipId = model.ApprenticeshipId,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>()
                });

            mocker.Mock<IApprenticeshipDataLockTriageService>()
                .Setup(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipDataLockTriageModel>()))
                .ReturnsAsync(() => new ApprenticeshipModel
                {
                    Id = dataLockTriageApprovedEvent.ApprenticeshipId
                });

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessApprenticeshipDataLockTriage(dataLockTriageApprovedEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev =>
                        ev.Id == dataLockTriageApprovedEvent.ApprenticeshipId),
                    It.IsAny<PublishOptions>()), Times.Once);

        }

        [Test]
        public async Task Process_Apprenticeship_Stopped_Correctly()
        {
            var stoppedEvent = new ApprenticeshipStoppedEvent()
            {
                ApprenticeshipId = 1,
                AppliedOn = DateTime.Today,
                StopDate = DateTime.Today
            };

            mocker.Mock<IApprenticeshipStoppedService>()
                .Setup(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipStoppedModel>()))
                .ReturnsAsync(() => new ApprenticeshipModel
                {
                    Id = stoppedEvent.ApprenticeshipId,
                });

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessStoppedApprenticeship(stoppedEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev =>
                        ev.Id == stoppedEvent.ApprenticeshipId),
                    It.IsAny<PublishOptions>()), Times.Once);

            mocker.Mock<IApprenticeshipStoppedService>()
                .Verify(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipStoppedModel>()), Times.Once);

        }


        [Test]
        public async Task Process_Apprenticeship_Stop_Date_Changed_Correctly()
        {
            var stopDateChangedEvent = new ApprenticeshipStopDateChangedEvent()
            {
                ApprenticeshipId = 1,
                ChangedOn = DateTime.Today,
                StopDate = DateTime.Today
            };

            mocker.Mock<IApprenticeshipStoppedService>()
                .Setup(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipStoppedModel>()))
                .ReturnsAsync(() => new ApprenticeshipModel
                {
                    Id = stopDateChangedEvent.ApprenticeshipId,
                });

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessStopDateChange(stopDateChangedEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev => ev.Id == stopDateChangedEvent.ApprenticeshipId), It.IsAny<PublishOptions>()), Times.Once);

            mocker.Mock<IApprenticeshipStoppedService>()
                .Verify(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipStoppedModel>()), Times.Once);

        }

        [Test]
        public async Task Process_Apprenticeship_Paused_Correctly()
        {
            var apprenticeshipPausedEvent = new ApprenticeshipPausedEvent()
            {
                ApprenticeshipId = 1,
                PausedOn = DateTime.Today
            };

            mocker.Mock<IApprenticeshipPauseService>()
                .Setup(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipPausedModel>()))
                .ReturnsAsync(() => new ApprenticeshipModel
                {
                    Id = apprenticeshipPausedEvent.ApprenticeshipId,
                });

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessPausedApprenticeship(apprenticeshipPausedEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev => ev.Id == apprenticeshipPausedEvent.ApprenticeshipId), It.IsAny<PublishOptions>()), Times.Once);

            mocker.Mock<IApprenticeshipPauseService>()
                .Verify(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipPausedModel>()), Times.Once);

        }

        [Test]
        public async Task Process_Apprenticeship_Resumed_Correctly()
        {
            var apprenticeshipResumedEvent = new ApprenticeshipResumedEvent()
            {
                ApprenticeshipId = 1,
                ResumedOn = DateTime.Today
            };

            mocker.Mock<IApprenticeshipResumedService>()
                .Setup(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipResumedModel>()))
                .ReturnsAsync(() => new ApprenticeshipModel
                {
                    Id = apprenticeshipResumedEvent.ApprenticeshipId,
                });

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessResumedApprenticeship(apprenticeshipResumedEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev => ev.Id == apprenticeshipResumedEvent.ApprenticeshipId), It.IsAny<PublishOptions>()), Times.Once);

            mocker.Mock<IApprenticeshipResumedService>()
                .Verify(svc => svc.UpdateApprenticeship(It.IsAny<UpdatedApprenticeshipResumedModel>()), Times.Once);

        }

        [Test]
        public async Task Process_Payment_Order_Change_Correctly()
        {
            var paymentOrderChangedEvent = new PaymentOrderChangedEvent()
            {
              AccountId = 1,
              PaymentOrder = new int[]{100, 200, 300}
            };
            
            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessPaymentOrderChange(paymentOrderChangedEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<EmployerChangedProviderPriority>(ev => ev.EmployerAccountId == paymentOrderChangedEvent.AccountId),
                    It.IsAny<PublishOptions>()),
                    Times.Once);
            
        }

    }
}