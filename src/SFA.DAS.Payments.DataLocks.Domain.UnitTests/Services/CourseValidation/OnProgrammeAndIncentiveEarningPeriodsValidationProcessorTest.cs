using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class OnProgrammeAndIncentiveEarningPeriodsValidationProcessorTest
    {
        private AutoMock mocker;
        private List<PriceEpisode> priceEpisodes;
        private List<ApprenticeshipModel> apprenticeships;
        private List<DataLockFailure> dataLockFailures;
        private LearningAim aim;
        private int AcademicYear = 1819;
        private readonly int ukprn = 123;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Provide<IStartDateValidator>(new Mock<IStartDateValidator>().Object);
            mocker.Provide<IOnProgrammeAndIncentiveStoppedValidator>(new Mock<IOnProgrammeAndIncentiveStoppedValidator>().Object);
            mocker.Provide<ICompletionStoppedValidator>( new Mock<ICompletionStoppedValidator>().Object);
            priceEpisodes = new List<PriceEpisode>
            {
                new PriceEpisode{Identifier = "pe-1"},
                new PriceEpisode{Identifier = "pe-2"},
                new PriceEpisode{Identifier = "pe-3"}
            };
            dataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    ApprenticeshipPriceEpisodeIds = new List<long>(),
                    DataLockError = DataLockErrorCode.DLOCK_03
                }
            };
            aim = new LearningAim();
        }

        [Test]
        public void MatchedPriceEpisodeIsReturnedInValidPeriods()
        {
             apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    },
                    EstimatedStartDate =new DateTime(2018, 8,1),
                    Status = ApprenticeshipStatus.Active
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 22,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 94},
                        new ApprenticeshipPriceEpisodeModel{Id = 95},
                        new ApprenticeshipPriceEpisodeModel{Id = 96},
                        new ApprenticeshipPriceEpisodeModel{Id = 97},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,1),
                    Status = ApprenticeshipStatus.Active
                }
            };
            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod
                {
                    Amount = 1,
                    PriceEpisodeIdentifier = "pe-1" ,
                    Period = 1
                }
                }.AsReadOnly()
            };


            mocker.Mock<IStartDateValidator>()
                .Setup(x => x.Validate(It.IsAny<PriceEpisode>(), It.IsAny<List<ApprenticeshipModel>>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));
            
            mocker.Mock<IOnProgrammeAndIncentiveStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<EarningPeriod>(),It.IsAny<int>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));
            
            mocker.Mock<ICompletionStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<PriceEpisode>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));
            
            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();
            
            var periods = earningProcessor.ValidatePeriods(
                ukprn,
                1,
                priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim,
                AcademicYear);

            periods.ValidPeriods.Count.Should().Be(1);
            periods.ValidPeriods.All(p => p.ApprenticeshipPriceEpisodeId == 90).Should().Be(true);
        }
        
        public void OnlyDLock09FailureIsReturnedPerDeliveryPeriod()
        {

        }

    }
}
