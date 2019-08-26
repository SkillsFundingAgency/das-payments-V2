using System;
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
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipUpdatedProcessorTests
    {
        private AutoMock mocker;
        private ApprenticeshipUpdated updatedApprenticeship;

        private IMapper mapper;

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<DataLocksProfile>()));
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Provide<IMapper>(mapper);
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

        [Test]
        public async Task Get_Apprenticeship_Update_Payments_Correctly()
        {
            mocker.Mock<IActorDataCache<List<ApprenticeshipModel>>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<List<ApprenticeshipModel>>(false, null));

            //Arrange
            updatedApprenticeship.Ukprn = 100;
            updatedApprenticeship.Uln = 200;

            var eventId = Guid.NewGuid();

            var earningModel = new ApprenticeshipContractType1EarningEvent
            {
                Ukprn = updatedApprenticeship.Ukprn,
                Learner = new Learner
                {
                    Uln = updatedApprenticeship.Uln
                },
                EventId = eventId,
                CollectionPeriod = new CollectionPeriod
                {
                    Period = 1,
                    AcademicYear = 1819
                },
                AgreementId = "1"
            };

            var earningKey = "key";
            
            mocker.Mock<IGenerateApprenticeshipEarningCacheKey>()
                .Setup(x => x.GenerateAct1EarningsKey(updatedApprenticeship.Ukprn, updatedApprenticeship.Uln))
                .Returns(earningKey)
                .Verifiable();

            mocker.Mock<IActorDataCache<ApprenticeshipContractType1EarningEvent>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<ApprenticeshipContractType1EarningEvent>(true, earningModel))
                .Verifiable();

            mocker.Mock<IDataLockProcessor>()
                .Setup(x => x.GetPaymentEvents(It.IsAny<ApprenticeshipContractType1EarningEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DataLockEvent>())
                .Verifiable();

            var processor = mocker.Create<ApprenticeshipUpdatedProcessor>();

            await processor.GetApprenticeshipUpdatePayments(updatedApprenticeship);

            mocker.Mock<IGenerateApprenticeshipEarningCacheKey>()
                .Verify(x => x.GenerateAct1EarningsKey(updatedApprenticeship.Ukprn, updatedApprenticeship.Uln), Times.Once);
            
            mocker.Mock<IActorDataCache<ApprenticeshipContractType1EarningEvent>>()
                .Verify(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
               
            mocker.Mock<IDataLockProcessor>()
                .Verify(x => x.GetPaymentEvents(
                        It.Is<ApprenticeshipContractType1EarningEvent>(o=> o.Ukprn == updatedApprenticeship.Ukprn && o.Learner.Uln == updatedApprenticeship.Uln),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }


        [Test]
        public async Task Get_Apprenticeship_Update_FunctionalSkill_Payments_Correctly()
        {
            mocker.Mock<IActorDataCache<List<ApprenticeshipModel>>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<List<ApprenticeshipModel>>(false, null));

            //Arrange
            updatedApprenticeship.Ukprn = 100;
            updatedApprenticeship.Uln = 200;

            var eventId = Guid.NewGuid();

            var earningModel = new Act1FunctionalSkillEarningsEvent
            {
                Ukprn = updatedApprenticeship.Ukprn,
                Learner = new Learner
                {
                    Uln = updatedApprenticeship.Uln,
                }
            };
            
            var earningKey = "key";

            mocker.Mock<IGenerateApprenticeshipEarningCacheKey>()
                .Setup(x => x.GenerateAct1FunctionalSkillEarningsKey(updatedApprenticeship.Ukprn, updatedApprenticeship.Uln))
                .Returns(earningKey)
                .Verifiable();

            mocker.Mock<IActorDataCache<Act1FunctionalSkillEarningsEvent>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<Act1FunctionalSkillEarningsEvent>(true, earningModel))
                .Verifiable();

            mocker.Mock<IDataLockProcessor>()
                .Setup(x => x.GetFunctionalSkillPaymentEvents(It.IsAny<Act1FunctionalSkillEarningsEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<FunctionalSkillDataLockEvent>())
                .Verifiable();

            var processor = mocker.Create<ApprenticeshipUpdatedProcessor>();

            await processor.GetApprenticeshipUpdateFunctionalSkillPayments(updatedApprenticeship);

            mocker.Mock<IActorDataCache<Act1FunctionalSkillEarningsEvent>>()
                .Verify(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

            mocker.Mock<IDataLockProcessor>()
                .Verify(x => x.GetFunctionalSkillPaymentEvents(
                        It.Is<Act1FunctionalSkillEarningsEvent>(o => o.Ukprn == updatedApprenticeship.Ukprn && o.Learner.Uln == updatedApprenticeship.Uln),
                        It.IsAny<CancellationToken>()),
                    Times.Once);

            mocker.Mock<IGenerateApprenticeshipEarningCacheKey>()
                .Verify(x => x.GenerateAct1FunctionalSkillEarningsKey(updatedApprenticeship.Ukprn, updatedApprenticeship.Uln), Times.Once);

        }
    }
}