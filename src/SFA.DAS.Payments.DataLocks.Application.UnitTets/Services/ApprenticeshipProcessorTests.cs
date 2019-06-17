using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Payments.DataLocks.Application.Services;
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
            mocker.Mock<IMessageSession>()
                .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .Returns(Task.CompletedTask);
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
                .Returns(Task.CompletedTask);

            var apprenticeshipProcessor = mocker.Create<ApprenticeshipProcessor>();
            await apprenticeshipProcessor.Process(approvalsEvent);

            mocker.Mock<IApprenticeshipService>()
                .Verify(svc => svc.NewApprenticeship(It.Is<ApprenticeshipModel>(model =>
                    model.AccountId == approvalsEvent.AccountId
                    && model.Id == approvalsEvent.ApprenticeshipId
                    && model.Ukprn == approvalsEvent.ProviderId
                    && model.Uln.ToString() == approvalsEvent.Uln)
                ), Times.Once);

            mocker.Mock<IMessageSession>()
                .Verify(svc => svc.Publish(It.Is<ApprenticeshipUpdated>(ev =>
                        ev.EmployerAccountId == approvalsEvent.AccountId
                        && ev.Id == approvalsEvent.ApprenticeshipId
                        && ev.Ukprn == approvalsEvent.ProviderId
                        && ev.Uln.ToString() == approvalsEvent.Uln),
                    It.IsAny<PublishOptions>()), Times.Once);
        }
    }
}