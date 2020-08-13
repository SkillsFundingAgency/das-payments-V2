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
                new List<(string identifier, PriceEpisodeStatus status)>(),
                new List<PriceEpisodeStatusChange>(), 2019);

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
            CommonTestSetup(repository, dataLock, periods, apprenticeships, dataLockFailures);
            var priceEpisode = dataLock.PriceEpisodes[0];

            dataLock.IlrFileName = "bob";
            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.New)
                },
                new List<PriceEpisodeStatusChange>(), dataLock.CollectionPeriod.AcademicYear);

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
            CommonTestSetup(repository, dataLock, periods, apprenticeships, dataLockFailures);
            var priceEpisode = dataLock.PriceEpisodes[0];

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.Updated)
                },
                new List<PriceEpisodeStatusChange>(), dataLock.CollectionPeriod.AcademicYear);

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

            CommonTestSetup(repository, dataLock, periods, apprenticeships, dataLockFailures);

            var priceEpisode = dataLock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 0;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.New)
                },
                new List<PriceEpisodeStatusChange>(), dataLock.CollectionPeriod.AcademicYear);

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

            CommonTestSetup(repository, dataLock, periods, apprenticeships, dataLockFailures);

            var priceEpisode = dataLock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 1;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.New)
                },
                new List<PriceEpisodeStatusChange>(), dataLock.CollectionPeriod.AcademicYear);

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
         List<ApprenticeshipModel> apprenticeships)
        {
            CommonTestSetup(repository, dataLock, periods, apprenticeships);

            var priceEpisode = dataLock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 1;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.New)
                },
                new List<PriceEpisodeStatusChange>(), dataLock.CollectionPeriod.AcademicYear);

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
            CommonTestSetup(repository, dataLock, periods, new List<ApprenticeshipModel> { apprenticeships }, dataLockFailures);
            var priceEpisode = dataLock.PriceEpisodes[0];
            periods[0].DataLockFailures[0].DataLockError = errorCode;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.New)
                },
                new List<PriceEpisodeStatusChange>(), dataLock.CollectionPeriod.AcademicYear);

            result.Should().NotBeEmpty();
            result[0].Errors.Should().ContainEquivalentOf(new
            {
                DataLockEventId = result[0].DataLock.DataLockEventId,
                ErrorCode = errorCode.ToString(),
                SystemDescription = description,
            });
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

            CommonTestSetup(repository, dataLockEvent, periods, apprenticeships, dataLockFailures);

            var result = await sut.Build(
                new List<DataLockEvent> { dataLockEvent },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (dataLockEvent.PriceEpisodes[0].Identifier,PriceEpisodeStatus.New)
                },
                new List<PriceEpisodeStatusChange>(), dataLockEvent.CollectionPeriod.AcademicYear);

            result.First().Periods.Should().ContainEquivalentOf(new
            {
                DataLockEventId = result.First().DataLock.DataLockEventId,
                IsPayable = false,
                TransactionTypesFlag = 1
            });

        }

        [Test, AutoDomainData]
        public async Task Build_PriceEpisode_Should_not_have_remove_event_twice(
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching dataLockEvent,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {
            CommonTestSetup(repository, dataLockEvent, periods, apprenticeships, dataLockFailures);

            var result = await sut.Build(
                new List<DataLockEvent> { dataLockEvent },
                new List<(string identifier, PriceEpisodeStatus status)>(),
                new List<PriceEpisodeStatusChange>(), dataLockEvent.CollectionPeriod.AcademicYear);

            result.Should().BeEmpty();

        }

        [Test, AutoDomainData]
        public async Task Build_PriceEpisode_Removed_Event(
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching dataLockEvent,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {

            CommonTestSetup(repository, dataLockEvent, periods, apprenticeships, dataLockFailures);
            var episodeStatusChange = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent
                    {
                        PriceEpisodeIdentifier = dataLockEvent.PriceEpisodes[0].Identifier,
                        Status = PriceEpisodeStatus.New,
                        UKPRN = dataLockEvent.Ukprn,
                        ULN = dataLockEvent.Learner.Uln,
                        AcademicYear = "1920"
                    }
                }
            };

            dataLockEvent.CollectionPeriod.AcademicYear = 1920;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLockEvent },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (dataLockEvent.PriceEpisodes[0].Identifier, PriceEpisodeStatus.Removed)
                },
                episodeStatusChange, dataLockEvent.CollectionPeriod.AcademicYear);

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new
                {
                    PriceEpisodeIdentifier = dataLockEvent.PriceEpisodes[0].Identifier,
                    Status = PriceEpisodeStatus.Removed,
                    UKPRN = dataLockEvent.Ukprn,
                    ULN = dataLockEvent.Learner.Uln
                },
            });

        }

        [Test, AutoDomainData]
        public async Task Build_Should_Not_Generate_Remove_Event_For_Previous_AcademicYear_PriceEpisode(
            [Frozen] Mock<IApprenticeshipRepository> repository,
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching dataLockEvent,
            List<EarningPeriod> periods,
            List<DataLockFailure> dataLockFailures,
            List<ApprenticeshipModel> apprenticeships)
        {

            CommonTestSetup(repository, dataLockEvent, periods, apprenticeships, dataLockFailures);
            var episodeStatusChange = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent
                    {
                        PriceEpisodeIdentifier = dataLockEvent.PriceEpisodes[0].Identifier,
                        Status = PriceEpisodeStatus.New,
                        UKPRN = dataLockEvent.Ukprn,
                        ULN = dataLockEvent.Learner.Uln,
                        AcademicYear = "1920"
                    }
                },
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent
                    {
                        PriceEpisodeIdentifier = dataLockEvent.PriceEpisodes[1].Identifier,
                        Status = PriceEpisodeStatus.New,
                        UKPRN = dataLockEvent.Ukprn,
                        ULN = dataLockEvent.Learner.Uln,
                        AcademicYear = "2021"
                    }
                }
            };

            dataLockEvent.CollectionPeriod.AcademicYear = 2021;

            var result = await sut.Build(
                new List<DataLockEvent> { dataLockEvent },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (dataLockEvent.PriceEpisodes[0].Identifier, PriceEpisodeStatus.Removed),
                    (dataLockEvent.PriceEpisodes[1].Identifier, PriceEpisodeStatus.Removed)
                },
                episodeStatusChange, dataLockEvent.CollectionPeriod.AcademicYear);

            result.Count.Should().Be(2);
            result.Select(s => s.DataLock).Should().BeEquivalentTo(new
            {
                PriceEpisodeIdentifier = dataLockEvent.PriceEpisodes[0].Identifier,
                Status = PriceEpisodeStatus.Updated,
                UKPRN = dataLockEvent.Ukprn,
                ULN = dataLockEvent.Learner.Uln,
                AcademicYear = "1920"
            }, new
            {
                PriceEpisodeIdentifier = dataLockEvent.PriceEpisodes[1].Identifier,
                Status = PriceEpisodeStatus.Removed,
                UKPRN = dataLockEvent.Ukprn,
                ULN = dataLockEvent.Learner.Uln,
                AcademicYear = "2021"
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

        [Test, AutoDomainData]
        public async Task Built_event_should_have_distinct_errors(
           [Frozen] Mock<IApprenticeshipRepository> repository,
           PriceEpisodeStatusChangeBuilder sut,
           EarningFailedDataLockMatching dataLock,
           EarningPeriod period,
           ApprenticeshipModel apprenticeships)
        {

           var periods = new List<EarningPeriod>{ period };
            
            var dLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    ApprenticeshipId = apprenticeships.Id,
                    DataLockError = DataLockErrorCode.DLOCK_03,
                    ApprenticeshipPriceEpisodeIds = new List<long> {apprenticeships.ApprenticeshipPriceEpisodes[0].Id}
                },
                new DataLockFailure
                {
                    ApprenticeshipId = apprenticeships.Id,
                    DataLockError = DataLockErrorCode.DLOCK_07,
                    ApprenticeshipPriceEpisodeIds = apprenticeships.ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                },
                new DataLockFailure
                {
                    ApprenticeshipId = apprenticeships.Id,
                    DataLockError = DataLockErrorCode.DLOCK_06,
                    ApprenticeshipPriceEpisodeIds = apprenticeships.ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                },
            };
            
            dataLock.OnProgrammeEarnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    Type = OnProgrammeEarningType.Learning,

                },
                new OnProgrammeEarning
                {
                    Type = OnProgrammeEarningType.Completion,
                }
            };

            CommonTestSetup(repository, dataLock, periods, new List<ApprenticeshipModel> { apprenticeships }, dLockFailures);


            var priceEpisode = dataLock.PriceEpisodes[0];

            dataLock.OnProgrammeEarnings[1].Periods = periods.AsReadOnly();


            var result = await sut.Build(
                new List<DataLockEvent> { dataLock },
                new List<(string identifier, PriceEpisodeStatus status)>
                {
                    (priceEpisode.Identifier, PriceEpisodeStatus.New)
                },
                new List<PriceEpisodeStatusChange>(), dataLock.CollectionPeriod.AcademicYear);

            result.Should().NotBeEmpty();
            result[0].Errors.Should().HaveCount(3);
            result[0].Errors.Should().BeEquivalentTo(new List<LegacyDataLockEventError>
            {
                new LegacyDataLockEventError
                {
                    DataLockEventId = result[0].DataLock.DataLockEventId,
                    ErrorCode = DataLockErrorCode.DLOCK_03.ToString(),
                },
                new LegacyDataLockEventError
                {
                    DataLockEventId = result[0].DataLock.DataLockEventId,
                    ErrorCode = DataLockErrorCode.DLOCK_07.ToString(),
                },
                new LegacyDataLockEventError
                {
                    DataLockEventId = result[0].DataLock.DataLockEventId,
                    ErrorCode = DataLockErrorCode.DLOCK_06.ToString(),
                },
            }, options => options.Excluding(o => o.SystemDescription));

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

        private static void CommonTestSetup(Mock<IApprenticeshipRepository> repository,
            DataLockEvent dataLock,
            List<EarningPeriod> periods,
            List<ApprenticeshipModel> apprenticeships,
            List<DataLockFailure> dataLockFailures = null
            )
        {
            repository
                .Setup(x => x.Get(It.IsAny<List<long>>(), CancellationToken.None))
                .Returns(Task.FromResult(apprenticeships));
            dataLock.IlrFileName = "bob";

            apprenticeships[0].ApprenticeshipPriceEpisodes.ForEach(x => x.ApprenticeshipId = apprenticeships[0].Id);
            periods[0].ApprenticeshipId = apprenticeships[0].Id;
            periods[0].ApprenticeshipPriceEpisodeId = apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id;
            periods[0].DataLockFailures = dataLock is PayableEarningEvent ? null : dataLockFailures;
            periods[0].PriceEpisodeIdentifier = dataLock.PriceEpisodes[0].Identifier;

            dataLock.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Learning;
            dataLock.OnProgrammeEarnings[0].Periods = periods.AsReadOnly();

            if (dataLock is EarningFailedDataLockMatching)
            {
                dataLockFailures.ForEach(x =>
                          {
                              x.ApprenticeshipId = apprenticeships[0].Id;
                              x.ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(o => o.Id).ToList();
                          });
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
                var fixture = new Fixture();
                fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
                return fixture;
            }
        }

    }


}
