using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class PriceEpisodeStatusChangeBuilder
    {
        public List<PriceEpisodeStatusChange> Build(List<DataLockEvent> dataLockEvents, List<(string identifier, PriceEpisodeStatus status)> priceEpisodeChanges)
        {
            //var priceEpisodesWithDatalocks = dataLockEvents
            //    .SelectMany(
            //        dl => dl.PriceEpisodes, 
            //        (datalock, priceEpisode) => (datalock, priceEpisode));

            //var changes = priceEpisodeChanges
            //    .LeftOuterJoin(priceEpisodesWithDatalocks, 
            //        left => left.identifier,
            //        right => right.priceEpisode.Identifier,
            //        (left, right) => 
            //            (left.identifier, right.priceEpisode, right.datalock, left.status))
            //    .ToList();

            //return changes.Select(MapPriceEpisodeStatusChange).ToList();
            var events = new List<PriceEpisodeStatusChange>();

            foreach (var priceEpisode in dataLockEvents.SelectMany(x => x.PriceEpisodes).Distinct())
            {
                var allDataLocks = dataLockEvents.Where(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisode.Identifier));

                var priceEpisodeErrors = allDataLocks
                    .OfType<EarningFailedDataLockMatching>()
                    .SelectMany(x => x.OnProgrammeEarnings)
                    .SelectMany(x => x.Periods)
                    .Where(p => p.PriceEpisodeIdentifier.Equals(priceEpisode.Identifier, StringComparison.InvariantCultureIgnoreCase))
                    .SelectMany(p => p.DataLockFailures)
                    .ToList();

                var apprenticeships = dataLockEvents
                    .SelectMany(x => x.OnProgrammeEarnings)
                    .SelectMany(x => x.Periods)
                    .GroupBy(x => x.ApprenticeshipId);

                foreach (var apprenticeship in apprenticeships)
                {
                    var priceEpisodeChange = priceEpisodeChanges.FirstOrDefault(x => x.identifier == priceEpisode.Identifier);

                    var evt = MapPriceEpisodeStatusChange(
                        apprenticeship.FirstOrDefault(),
                        allDataLocks.First(),
                        priceEpisodeErrors,
                        priceEpisodeChange.status,
                        priceEpisode);

                    events.Add(evt);
                }
            }

            return events;
        }

        private static PriceEpisodeStatusChange MapPriceEpisodeStatusChange(
            EarningPeriod apprenticeshipEarning,
            DataLockEvent datalock,
            List<DataLockFailure> errors,
            PriceEpisodeStatus status,
            PriceEpisode priceEpisode)
        {

            var period = datalock.OnProgrammeEarnings
                .SelectMany(x => x.Periods)
                .Distinct()
                .FirstOrDefault();

            var hasTnp3 = priceEpisode.TotalNegotiatedPrice3 > 0;

            return new PriceEpisodeStatusChange
            {
                JobId = datalock.JobId,
                Ukprn = datalock.Ukprn,
                CollectionPeriod = datalock.CollectionPeriod,

                DataLock = new LegacyDataLockEvent
                {
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    Status = status,

                    AcademicYear = datalock.CollectionPeriod.AcademicYear.ToString(),
                    UKPRN = datalock.Ukprn,
                    DataLockEventId = datalock.EventId,
                    EventSource = 1, // submission
                    HasErrors = errors.Any(),
                    ULN = datalock.Learner.Uln,
                    ProcessDateTime = DateTime.UtcNow,
                    LearnRefNumber = datalock.Learner.ReferenceNumber,
                    IlrFrameworkCode = datalock.LearningAim.FrameworkCode,
                    IlrPathwayCode = datalock.LearningAim.PathwayCode,
                    IlrProgrammeType = datalock.LearningAim.ProgrammeType,
                    IlrStandardCode = datalock.LearningAim.StandardCode,
                    SubmittedDateTime = datalock.IlrSubmissionDateTime,

                    CommitmentId = apprenticeshipEarning?.ApprenticeshipId ?? 0,
                    EmployerAccountId = apprenticeshipEarning?.AccountId ?? 0,

                    AimSeqNumber = datalock.LearningAim.SequenceNumber,
                    IlrPriceEffectiveFromDate = priceEpisode.EffectiveTotalNegotiatedPriceStartDate,
                    IlrPriceEffectiveToDate = priceEpisode.ActualEndDate.GetValueOrDefault(priceEpisode.PlannedEndDate),
                    IlrEndpointAssessorPrice = hasTnp3 ? priceEpisode.TotalNegotiatedPrice4 : priceEpisode.TotalNegotiatedPrice2,
                    IlrFileName = TrimUkprnFromIlrFileNameLimitToValidLength(datalock.IlrFileName),
                    IlrStartDate = priceEpisode.CourseStartDate,
                    IlrTrainingPrice = hasTnp3 ? priceEpisode.TotalNegotiatedPrice3 : priceEpisode.TotalNegotiatedPrice1,
                },
            };
        }

        internal static string TrimUkprnFromIlrFileNameLimitToValidLength(string input)
        {
            const int validLength = 50;

            if (input == null) return string.Empty;

            if (input.Length > 9)
            {
                input = input.Substring(9);

                if (input.Length > validLength)
                {
                    return input.Substring(0, validLength);
                }
            }

            return input;
        }

        //private static PriceEpisodeStatusChange MapPriceEpisodeStatusChange(
        //    (string priceEpisodeIdentifier,
        //     PriceEpisode priceEpisode,
        //     DataLockEvent datalock,
        //     PriceEpisodeStatus status) change)
        //    => MapPriceEpisodeStatusChange(
        //        change.datalock,
        //        change.status,
        //        change.priceEpisode);
    }
}
