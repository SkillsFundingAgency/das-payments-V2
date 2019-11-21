using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    public class PriceEpisodeStatusChangeBuilderTest
    {
        [Test, AutoDomainData]
        public async Task No_datalocks_or_current_price_episodes_builds_no_events(
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut)
        {
            var result = await sut.Build(
                new List<DataLockEvent>(),
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().BeEmpty();
        }

        [Test, AutoDomainData]
        public async Task Builds_an_add_event_with_data_from_datalock(
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent dataLock,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {

            CommonTestSetup(repository, dataLock, periods, dataLockFailures, apprenticeships);
            
            dataLock.IlrFileName = "bob";
            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            var priceEpisode = dataLock.PriceEpisodes[0];

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    AcademicYear = dataLock.CollectionPeriod.AcademicYear.ToString(),
                    UKPRN = dataLock.Ukprn,
                    EventSource = 1,
                    HasErrors = false,
                    ULN = dataLock.Learner.Uln,
                    LearnRefNumber = dataLock.Learner.ReferenceNumber,
                    IlrFrameworkCode = dataLock.LearningAim.FrameworkCode,
                    IlrPathwayCode = dataLock.LearningAim.PathwayCode,
                    IlrProgrammeType = dataLock.LearningAim.ProgrammeType,
                    IlrStandardCode = dataLock.LearningAim.StandardCode,
                    SubmittedDateTime = dataLock.IlrSubmissionDateTime,
                    AimSeqNumber = dataLock.LearningAim.SequenceNumber,
                    IlrPriceEffectiveFromDate = priceEpisode.EffectiveTotalNegotiatedPriceStartDate,
                    IlrPriceEffectiveToDate = priceEpisode.ActualEndDate.GetValueOrDefault(priceEpisode.PlannedEndDate),
                    IlrFileName = "bob",
                    IlrStartDate = priceEpisode.CourseStartDate,
                }
            });
        }

        [Test, AutoDomainData]
        public async Task Builds_an_add_event_with_status(
            PriceEpisodeStatusChangeBuilder sut,
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PayableEarningEvent dataLock,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {
            CommonTestSetup(repository, dataLock, periods, dataLockFailures, apprenticeships);
            var priceEpisode = dataLock.PriceEpisodes[0];

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.Updated)
                });
            
            result.Should().ContainEquivalentOf(new
            {
                DataLock = new
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    Status = PriceEpisodeStatus.Updated,
                }
            });
        }

           [Test, AutoDomainData]
        public async Task Builds_an_add_event_without_tnp3(
            PriceEpisodeStatusChangeBuilder sut,
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PayableEarningEvent dataLock,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {

            CommonTestSetup(repository, dataLock, periods, dataLockFailures, apprenticeships);

            var priceEpisode = dataLock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 0;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    IlrEndpointAssessorPrice = priceEpisode.TotalNegotiatedPrice2,
                    IlrTrainingPrice = priceEpisode.TotalNegotiatedPrice1,
                }
            });
        }

           [Test, AutoDomainData]
        public async Task Builds_an_add_event_with_tnp3(
      [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent dataLock,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {

            CommonTestSetup(repository, dataLock, periods, dataLockFailures, apprenticeships);

            var priceEpisode = dataLock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 1;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    IlrEndpointAssessorPrice = priceEpisode.TotalNegotiatedPrice4,
                    IlrTrainingPrice = priceEpisode.TotalNegotiatedPrice3,
                }
            });
        }

           [Test, AutoDomainData]
        public async Task Builds_an_add_event_with_no_errors(
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent dataLock,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {
            CommonTestSetup(repository, dataLock, periods, dataLockFailures, apprenticeships);

            var priceEpisode = dataLock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 1;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new { PriceEpisodeIdentifier = priceEpisode.Identifier },
                Errors = new LegacyDataLockEventError[0]
            });
        }

        [Test]
        [InlineDomainAutoData(DataLockErrorCode.DLOCK_03, "No matching record found in the employer digital account for the standard code")]
        [InlineDomainAutoData(DataLockErrorCode.DLOCK_04, "No matching record found in the employer digital account for the framework code")]
        [InlineDomainAutoData(DataLockErrorCode.DLOCK_05, "No matching record found in the employer digital account for the programme type")]
        [InlineDomainAutoData(DataLockErrorCode.DLOCK_06, "No matching record found in the employer digital account for the pathway code")]
        [InlineDomainAutoData(DataLockErrorCode.DLOCK_07, "No matching record found in the employer digital account for the negotiated cost of training")]
        [InlineDomainAutoData(DataLockErrorCode.DLOCK_08, "Multiple matching records found in the employer digital account")]
        [InlineDomainAutoData(DataLockErrorCode.DLOCK_09, "The start date for this negotiated price is before the corresponding price start date in the employer digital account")]
        public async Task Builds_an_event_with_errors(
            DataLockErrorCode errorCode,
            string description,
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching dataLock,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            ApprenticeshipModel apprenticeships)
        {
            CommonTestSetup(repository, dataLock, periods, dataLockFailures, new List<ApprenticeshipModel>{apprenticeships});
            var priceEpisode = dataLock.PriceEpisodes[0];
            periods[0].DataLockFailures[0].DataLockError = errorCode;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().NotBeEmpty();
            result[0].Errors.Should().ContainEquivalentOf(new
            {
                DataLockEventId = result[0].DataLock.DataLockEventId,
                ErrorCode = errorCode.ToString(),
                SystemDescription = description,
            });
        }
        
           [Test, AutoDomainData]
        public async Task Builds_one_for_each_apprenticeship_within_price_episode(
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent dataLock)
        {
            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            var numPriceEpisodes = dataLock.PriceEpisodes.Count();
            var numApprenticeshipIds = dataLock
                .OnProgrammeEarnings
                .SelectMany(x => x.Periods)
                .Select(x => x.ApprenticeshipId)
                .Distinct()
                .Count();

            result.Should().HaveCount(numApprenticeshipIds * numPriceEpisodes);
        }

        [Test, AutoDomainData]
        public async Task Build_Period_For_Apprenticeship_And_PriceEpisode(
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching dataLockEvent,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {

            CommonTestSetup(repository, dataLockEvent, periods, dataLockFailures, apprenticeships);

            var result = await sut.Build(
                new List<DataLockEvent> { dataLockEvent },
                new List<(string identifier, PriceEpisodeStatus status)>{(dataLockEvent.PriceEpisodes[0].Identifier,PriceEpisodeStatus.New) });

            result.First().Periods.Should().ContainEquivalentOf(new
            {
                DataLockEventId = result.First().DataLock.DataLockEventId,
                IsPayable = true,
                TransactionTypesFlag = 1
            });

        }


           [Test, AutoDomainData]
        public void PriceEpisodesAreDistinctByIdentifier()
        {
            var episode1 = new PriceEpisode { Identifier = "hi" };
            var episode2 = new PriceEpisode { Identifier = "hi" };
            var list = new[] { episode1, episode2 };
            list.Distinct().Should().HaveCount(1);
        }


        public class AutoDomainDataAttribute : AutoDataAttribute
        {
            public AutoDomainDataAttribute()
                : base(Customise)
            {
            }

            private static IFixture Customise()
            {
                var dbName = Guid.NewGuid().ToString();
                var fixture = new Fixture();
                fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
                return fixture;
            }
        }

        public class InlineDomainAutoDataAttribute : InlineAutoDataAttribute
        {
            public InlineDomainAutoDataAttribute(params object[] values)
                : base(Customise, values)
            {
            }
            private static IFixture Customise()
            {
                var dbName = Guid.NewGuid().ToString();
                var fixture = new Fixture();
                fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
                return fixture;
            }
        }
        
        private static void CommonTestSetup(Mock<IApprenticeshipRepository> repository,
            DataLockEvent dataLock,
            List<EarningPeriod> periods, List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {
            repository
                .Setup(x => x.Get(It.IsAny<List<long>>(), CancellationToken.None))
                .Returns(Task.FromResult(apprenticeships));
            dataLock.IlrFileName = "bob";

            apprenticeships[0].ApprenticeshipPriceEpisodes.ForEach(x => x.ApprenticeshipId = apprenticeships[0].Id);
            periods[0].ApprenticeshipId = apprenticeships[0].Id;
            periods[0].ApprenticeshipPriceEpisodeId = apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id;
            periods[0].DataLockFailures = dataLockFailures;
            periods[0].PriceEpisodeIdentifier = dataLock.PriceEpisodes[0].Identifier;
            dataLock.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Learning;
            dataLock.OnProgrammeEarnings[0].Periods = periods.AsReadOnly();
            dataLockFailures.ForEach(x =>
            {
                x.ApprenticeshipId = apprenticeships[0].Id;
                x.ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(o => o.Id).ToList();
            });

        }


    }


}
