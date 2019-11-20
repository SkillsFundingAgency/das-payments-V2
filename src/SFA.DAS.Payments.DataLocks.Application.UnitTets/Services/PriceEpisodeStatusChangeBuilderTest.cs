using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    public class PriceEpisodeStatusChangeBuilderTest
    {
        [Test, AutoData]
        public void No_datalocks_or_current_price_episodes_builds_no_events(
            PriceEpisodeStatusChangeBuilder sut)
        {
            var result = sut.Build(
                new List<DataLockEvent>(),
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Builds_an_add_event_with_data_from_datalock(
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent datalock)
        {
            datalock.IlrFileName = "bob";

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            var priceEpisode = datalock.PriceEpisodes[0];

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    AcademicYear = datalock.CollectionPeriod.AcademicYear.ToString(),
                    UKPRN = datalock.Ukprn,
                    DataLockEventId = datalock.EventId,
                    EventSource = 1,
                    HasErrors = false,
                    ULN = datalock.Learner.Uln,
                    LearnRefNumber = datalock.Learner.ReferenceNumber,
                    IlrFrameworkCode = datalock.LearningAim.FrameworkCode,
                    IlrPathwayCode = datalock.LearningAim.PathwayCode,
                    IlrProgrammeType = datalock.LearningAim.ProgrammeType,
                    IlrStandardCode = datalock.LearningAim.StandardCode,
                    SubmittedDateTime = datalock.IlrSubmissionDateTime,
                    AimSeqNumber = datalock.LearningAim.SequenceNumber,
                    IlrPriceEffectiveFromDate = priceEpisode.EffectiveTotalNegotiatedPriceStartDate,
                    IlrPriceEffectiveToDate = priceEpisode.ActualEndDate.GetValueOrDefault(priceEpisode.PlannedEndDate),
                    IlrFileName = "bob",
                    IlrStartDate = priceEpisode.CourseStartDate,
                }
            });
        }

        [Test, AutoData]
        public void Builds_an_add_event_with_status(
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent datalock)
        {
            datalock.IlrFileName = "bob";
            var priceEpisode = datalock.PriceEpisodes[0];

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
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

        [Test, AutoData]
        public void Builds_an_add_event_without_tnp3(
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent datalock)
        {
            var priceEpisode = datalock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 0;

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
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

        [Test, AutoData]
        public void Builds_an_add_event_with_tnp3(
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent datalock)
        {
            var priceEpisode = datalock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 1;

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
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

        [Test, AutoData]
        public void Builds_an_add_event_with_no_errors(
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent datalock)
        {
            var priceEpisode = datalock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 1;

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new { PriceEpisodeIdentifier = priceEpisode.Identifier },
                Errors = new LegacyDataLockEventError[0]
            });
        }

        [Test]
        [InlineAutoData(DataLockErrorCode.DLOCK_01, "No matching record found in an employer digital account for the UKPRN")]
        [InlineAutoData(DataLockErrorCode.DLOCK_03, "No matching record found in the employer digital account for the standard code")]
        [InlineAutoData(DataLockErrorCode.DLOCK_04, "No matching record found in the employer digital account for the framework code")]
        [InlineAutoData(DataLockErrorCode.DLOCK_05, "No matching record found in the employer digital account for the programme type")]
        [InlineAutoData(DataLockErrorCode.DLOCK_06, "No matching record found in the employer digital account for the pathway code")]
        [InlineAutoData(DataLockErrorCode.DLOCK_07, "No matching record found in the employer digital account for the negotiated cost of training")]
        [InlineAutoData(DataLockErrorCode.DLOCK_08, "Multiple matching records found in the employer digital account")]
        [InlineAutoData(DataLockErrorCode.DLOCK_09, "The start date for this negotiated price is before the corresponding price start date in the employer digital account")]
        [InlineAutoData(DataLockErrorCode.DLOCK_10, "The employer has stopped payments for this apprentice")]
        [InlineAutoData(DataLockErrorCode.DLOCK_11, "The employer is not currently a levy payer")]
        [InlineAutoData(DataLockErrorCode.DLOCK_12, "DLOCK_12")]
        public void Builds_an_event_with_errors(
            DataLockErrorCode errorCode,
            string description,
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching datalock)
        {
            var priceEpisode = datalock.PriceEpisodes[0];
            datalock.OnProgrammeEarnings[0].Periods[0].PriceEpisodeIdentifier = priceEpisode.Identifier;
            datalock.OnProgrammeEarnings[0].Periods[0].DataLockFailures[0].DataLockError = errorCode;

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().NotBeEmpty();
            result.First().Errors.Should().ContainEquivalentOf(new
            {
                DataLockEventId = datalock.EventId,
                ErrorCode = errorCode.ToString(),
                SystemDescription = description,
            });
        }

        [Test, AutoData]
        public void Builds_an_event_errors_with_datalock_eventid(
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching datalock)
        {
            datalock.OnProgrammeEarnings[0].Periods[0].PriceEpisodeIdentifier = datalock.PriceEpisodes[0].Identifier;

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().NotBeEmpty();
            result.First().Errors.Should().OnlyContain(x => x.DataLockEventId == datalock.EventId);
        }

        [Test, AutoData]
        public void Builds_an_event_with_(
        PriceEpisodeStatusChangeBuilder sut,
        EarningFailedDataLockMatching datalock)
        {
            var priceEpisode = datalock.PriceEpisodes[0];
            datalock.OnProgrammeEarnings[0].Periods[0].PriceEpisodeIdentifier = priceEpisode.Identifier;
            datalock.OnProgrammeEarnings[0].Periods[0].DataLockFailures[0].DataLockError = DataLockErrorCode.DLOCK_01;

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().NotBeEmpty();
            result.First().Errors.Should().ContainEquivalentOf(new
            {
                DataLockEventId = datalock.EventId,
                ErrorCode = DataLockErrorCode.DLOCK_01.ToString(),
                SystemDescription = "No matching record found in an employer digital account for the UKPRN",
            });
        }

        [Test, AutoData]
        public void Builds_one_even_per_price(
            PriceEpisodeStatusChangeBuilder sut,
            PayableEarningEvent datalock)
        {
            var result = sut.Build(
                new List<DataLockEvent> { datalock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            var numPriceEpisodes = datalock.PriceEpisodes.Count();
            var numPeriods = datalock
                .OnProgrammeEarnings
                .SelectMany(x => x.Periods)
                .Select(x => x.ApprenticeshipId)
                .Distinct()
                .Count();

            result.Should().HaveCount(numPriceEpisodes * numPriceEpisodes);
        }

        [Test, AutoData]
        public void PriceEpisodesAreDistinctByIdentifier()
        {
            var episode1 = new PriceEpisode { Identifier = "hi" };
            var episode2 = new PriceEpisode { Identifier = "hi" };
            var list = new[] { episode1, episode2 };
            list.Distinct().Should().HaveCount(1);
        }
    }
}
