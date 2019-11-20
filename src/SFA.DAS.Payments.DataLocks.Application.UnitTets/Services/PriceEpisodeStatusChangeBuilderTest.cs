using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
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

        [Test, AutoData]
        public void Builds_an_add_event_with_errors(
            PriceEpisodeStatusChangeBuilder sut,
            EarningFailedDataLockMatching datalock)
        {
            var priceEpisode = datalock.PriceEpisodes[0];
            priceEpisode.TotalNegotiatedPrice3 = 1;

            var result = sut.Build(
                new List<DataLockEvent> { datalock },
                new List<(string identifier, PriceEpisodeStatus status)>());

            result.Should().ContainEquivalentOf(new
            {
                DataLock = new { PriceEpisodeIdentifier = priceEpisode.Identifier },
                Errors = new []
                {
                    new { DataLockEventId = "" },
                }
            });
        }

        //[Test, AutoData]
        //public void Builds_one_even_per_price(
        //    PriceEpisodeStatusChangeBuilder sut,
        //    PayableEarningEvent datalock)
        //{
        //    datalock.IlrFileName = "bob";

        //    var result = sut.Build(
        //        new List<DataLockEvent> { datalock },
        //        new List<(string identifier, PriceEpisodeStatus status)>());

        //    var priceEpisode = datalock.PriceEpisodes[0];

        //    result.Should().ContainEquivalentOf(new
        //    {
        //        DataLock = new
        //        {
        //            PriceEpisodeIdentifier = priceEpisode.Identifier,
        //            AcademicYear = datalock.CollectionPeriod.AcademicYear.ToString(),
        //            UKPRN = datalock.Ukprn,
        //            DataLockEventId = datalock.EventId,
        //            EventSource = 1,
        //            HasErrors = false,
        //            ULN = datalock.Learner.Uln,
        //            LearnRefNumber = datalock.Learner.ReferenceNumber,
        //            IlrFrameworkCode = datalock.LearningAim.FrameworkCode,
        //            IlrPathwayCode = datalock.LearningAim.PathwayCode,
        //            IlrProgrammeType = datalock.LearningAim.ProgrammeType,
        //            IlrStandardCode = datalock.LearningAim.StandardCode,
        //            SubmittedDateTime = datalock.IlrSubmissionDateTime,
        //            AimSeqNumber = datalock.LearningAim.SequenceNumber,
        //            IlrPriceEffectiveFromDate = priceEpisode.EffectiveTotalNegotiatedPriceStartDate,
        //            IlrPriceEffectiveToDate = priceEpisode.ActualEndDate.GetValueOrDefault(priceEpisode.PlannedEndDate),
        //            IlrFileName = "bob",
        //            IlrStartDate = priceEpisode.CourseStartDate,
        //        }
        //    });
        //}
    }
}
