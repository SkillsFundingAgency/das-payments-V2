using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Exceptions;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipProcessorTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var mapperConfiguration = new MapperConfiguration(expression =>
            {
                expression.AddProfile(typeof(DataLocksProfile));
            });
            mocker.Provide<IMapper>(new Mapper(mapperConfiguration));

            mocker.Mock<IEndpointInstance>()
                .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .Returns(Task.CompletedTask);
            mocker.Mock<IEndpointInstanceFactory>()
                .Setup(x => x.GetEndpointInstance())
                .ReturnsAsync(mocker.Mock<IEndpointInstance>().Object);
        }

        [Test]
        public async Task ProcessCreatedEvent_WithExistingApprenticeship_DoesNotFail()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                TransferSenderId = 123456,
            };

            mocker.Mock<IApprenticeshipService>().Setup(x => x.NewApprenticeship(It.Is<ApprenticeshipModel>(model =>
                model.AccountId == 12345 &&
                model.TransferSendingEmployerAccountId == 123456
            ))).Throws(new ApprenticeshipAlreadyExistsException(1));

            var sut = mocker.Create<ApprenticeshipProcessor>();

            Func<Task> action = async () => await sut.Process(approvalsEvent);
            action.Should().NotThrow();
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
                PriceEpisodes = new [] { new PriceEpisode { FromDate = DateTime.Today, Cost = 1000M } }
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
                PriceEpisodes = new [] { new PriceEpisode { FromDate = DateTime.Today, Cost = 1000M } }
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
                PriceEpisodes = new [] { new PriceEpisode { FromDate = DateTime.Today, Cost = 1000M } }
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
                TrainingCode = "98",
                TrainingType = ProgrammeType.Standard,
                PriceEpisodes = new []
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

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessUpdatedApprenticeship(approvalsEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev =>
                        ev.Id == approvalsEvent.ApprenticeshipId
                        && ev.Uln.ToString() == approvalsEvent.Uln),
                    It.IsAny<PublishOptions>()), Times.Once);
        }
        
        [Test]
        public async Task Process_Apprenticeship_DataLock_Triage()
        {
            var dataLockTriageApprovedEvent = new DataLockTriageApprovedEvent()
            {
                ApprenticeshipId = 1,
                ApprovedOn = DateTime.Today,
                TrainingCode = "98",
                TrainingType = ProgrammeType.Standard,
                PriceEpisodes = new []
                {
                    new PriceEpisode
                    {
                        FromDate = DateTime.Today,
                        ToDate = DateTime.Today.AddYears(1),
                        Cost = 1000m
                    }
                }
            };

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
              PaymentOrder = new []{300, 100, 200}
            };
            
            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessPaymentOrderChange(paymentOrderChangedEvent);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(
                        It.Is<EmployerChangedProviderPriority>(ev => ev.EmployerAccountId == paymentOrderChangedEvent.AccountId &&
                                                                     ev.OrderedProviders.Count == 3 &&
                                                                     ev.OrderedProviders[0] == paymentOrderChangedEvent.PaymentOrder[0] &&
                                                                     ev.OrderedProviders[1] == paymentOrderChangedEvent.PaymentOrder[1] &&
                                                                     ev.OrderedProviders[2] == paymentOrderChangedEvent.PaymentOrder[2] ),
                    It.IsAny<PublishOptions>()),
                    Times.Once);
        }

        [Test]
        public async Task Process_Apprenticeship_For_NonLevyPayer_Employer_Correctly()
        {
            var apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id =100,
                    AccountId = 1,
                    IsLevyPayer =  false
                }
            };

            mocker.Mock<IApprenticeshipService>()
                .Setup(svc => svc.GetUpdatedApprenticeshipEmployerIsLevyPayerFlag(apprenticeships[0].AccountId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeships);
            
            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessApprenticeshipForNonLevyPayerEmployer(1);
            
            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev => ev.Id == apprenticeships[0].Id &&
                                                                              ev.EmployerAccountId == apprenticeships[0].AccountId &&
                                                                              ev.IsLevyPayer == apprenticeships[0].IsLevyPayer),
                    It.IsAny<PublishOptions>()),
                    Times.Once);
        }

        [Test]
        public async Task Apprenticeship_Update_Message_Is_Not_Published_If_No_Apprenticeship_Is_Found()
        {
            var apprenticeships = new List<ApprenticeshipModel>();
         
            mocker.Mock<IApprenticeshipService>()
                .Setup(svc => svc.GetUpdatedApprenticeshipEmployerIsLevyPayerFlag(1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeships);

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.ProcessApprenticeshipForNonLevyPayerEmployer(1);

            mocker.Mock<IEndpointInstance>()
                .Verify(svc => svc.Publish(It.IsAny<ApprenticeshipUpdated>(), It.IsAny<PublishOptions>()), Times.Never);
        }
    }
}