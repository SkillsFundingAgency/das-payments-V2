using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class PriceEpisodeStatusChangeBuilder
    {
        public List<PriceEpisodeStatusChange> Build(List<DataLockEvent> dataLockEvents, List<(string identifier, PriceEpisodeStatus status)> priceEpisodeChanges)
        {
            var present = BuildPresentEvents(dataLockEvents, priceEpisodeChanges);

            var removed = BuildRemovedEvents(priceEpisodeChanges);

            return present.Union(removed).ToList();
        }

        private static List<PriceEpisodeStatusChange> BuildPresentEvents(List<DataLockEvent> dataLockEvents, List<(string identifier, PriceEpisodeStatus status)> priceEpisodeChanges)
        {
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

                var apprenticeshipEarnings = dataLockEvents
                    .SelectMany(x => x.OnProgrammeEarnings)
                    .SelectMany(x => x.Periods)
                    .GroupBy(x => x.ApprenticeshipId);

                foreach (var apprenticeshipEarning in apprenticeshipEarnings)
                {
                    var priceEpisodeChange = priceEpisodeChanges.FirstOrDefault(x => x.identifier == priceEpisode.Identifier);

                    var evt = MapPriceEpisodeStatusChange(
                        apprenticeshipEarning.FirstOrDefault(),
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

                CommitmentVersions = BuildCommitmentVersions(datalock.EventId, GetApprenticeshipModel(apprenticeshipEarning.ApprenticeshipId)),

                //Period = BuildPeriods(datalock.OnProgrammeEarnings),

                Errors = BuildCommitmentErrors(datalock.EventId, errors),
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

        private static LegacyDataLockEventCommitmentVersion[] BuildCommitmentVersions(Guid dataLockEventId, ApprenticeshipModel apprenticeship)
        {
            if (apprenticeship == null) return Array.Empty<LegacyDataLockEventCommitmentVersion>();

            return apprenticeship.ApprenticeshipPriceEpisodes
                .Select(apprenticeshipPriceEpisode => new LegacyDataLockEventCommitmentVersion
                {
                    DataLockEventId = dataLockEventId,
                    CommitmentVersion = $"{apprenticeship.Id}-{apprenticeshipPriceEpisode.Id}",
                    CommitmentEffectiveDate = apprenticeshipPriceEpisode.StartDate,
                    CommitmentFrameworkCode = apprenticeship.FrameworkCode,
                    CommitmentNegotiatedPrice = apprenticeshipPriceEpisode.Cost,
                    CommitmentPathwayCode = apprenticeship.PathwayCode,
                    CommitmentProgrammeType = apprenticeship.ProgrammeType,
                    CommitmentStandardCode = apprenticeship.StandardCode,
                    CommitmentStartDate = apprenticeship.EstimatedStartDate
                })
                .ToArray();
        }

        private static LegacyDataLockEventError[] BuildCommitmentErrors(Guid eventId, List<DataLockFailure> dataLockFailures)
        {
            return dataLockFailures
                .Distinct()
                .Select(x => new LegacyDataLockEventError
                {
                    DataLockEventId = eventId,
                    SystemDescription = GetDataLockDescription(x.DataLockError),
                    ErrorCode = x.DataLockError.ToString()
                })
                .ToArray();
        }

        private static string GetDataLockDescription(DataLockErrorCode dlockCode)
        {
            switch (dlockCode)
            {
                case DataLockErrorCode.DLOCK_01: return "No matching record found in an employer digital account for the UKPRN";
                case DataLockErrorCode.DLOCK_03: return "No matching record found in the employer digital account for the standard code";
                case DataLockErrorCode.DLOCK_04: return "No matching record found in the employer digital account for the framework code";
                case DataLockErrorCode.DLOCK_05: return "No matching record found in the employer digital account for the programme type";
                case DataLockErrorCode.DLOCK_06: return "No matching record found in the employer digital account for the pathway code";
                case DataLockErrorCode.DLOCK_07: return "No matching record found in the employer digital account for the negotiated cost of training";
                case DataLockErrorCode.DLOCK_08: return "Multiple matching records found in the employer digital account";
                case DataLockErrorCode.DLOCK_09: return "The start date for this negotiated price is before the corresponding price start date in the employer digital account";
                case DataLockErrorCode.DLOCK_10: return "The employer has stopped payments for this apprentice";
                case DataLockErrorCode.DLOCK_11: return "The employer is not currently a levy payer";
                case DataLockErrorCode.DLOCK_12: return "DLOCK_12";
                default: return dlockCode.ToString();
            }
        }

        private static LegacyDataLockEventPeriod[] BuildPeriods(DataLockEvent dataLock, List<OnProgrammeEarning> onProgrammeEarnings)
        {
            var collectionPeriod = dataLock.CollectionPeriod.Period;

            return onProgrammeEarnings.Select(x => 
                new LegacyDataLockEventPeriod
                {
                    DataLockEventId = dataLock.EventId,
                    //TransactionTypesFlag = GetTransactionTypeFlag(dataLock),
                    CollectionPeriodYear = dataLock.CollectionPeriod.AcademicYear,
                    CollectionPeriodName = $"{dataLock.CollectionPeriod.AcademicYear}-{collectionPeriod:D2}",
                    CollectionPeriodMonth = (collectionPeriod < 6) ? collectionPeriod + 7 : collectionPeriod - 5,
                    //IsPayable = !isError,
                    //CommitmentVersion = commitmentVersionId
                })
                .ToArray();
        }

        private int GetTransactionTypeFlag(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Learning:
                case TransactionType.OnProgramme16To18FrameworkUplift:
                case TransactionType.OnProgrammeMathsAndEnglish:
                case TransactionType.BalancingMathsAndEnglish:
                case TransactionType.LearningSupport:
                    return 1;

                case TransactionType.First16To18EmployerIncentive:
                case TransactionType.First16To18ProviderIncentive:
                case TransactionType.FirstDisadvantagePayment:
                    return 2;

                case TransactionType.Second16To18EmployerIncentive:
                case TransactionType.Second16To18ProviderIncentive:
                case TransactionType.SecondDisadvantagePayment:
                    return 3;

                case TransactionType.Completion:
                case TransactionType.Balancing:
                case TransactionType.Completion16To18FrameworkUplift:
                case TransactionType.Balancing16To18FrameworkUplift:
                    return 4;

                case TransactionType.CareLeaverApprenticePayment:
                    return 5;

                default:
                    throw new ArgumentException($"Transaction Type {transactionType} not supported.", nameof(transactionType));
            }
        }

        private static ApprenticeshipModel GetApprenticeshipModel(long? apprenticeshipId)
        {
            return null;
        }

        private static List<PriceEpisodeStatusChange> BuildRemovedEvents(List<(string identifier, PriceEpisodeStatus status)> priceEpisodeChanges)
        {
            return priceEpisodeChanges
                .Where(x => x.status == PriceEpisodeStatus.Removed)
                .Select(MapRemovedPriceEpisodeStatusChange)
                .ToList();
        }

        private static PriceEpisodeStatusChange MapRemovedPriceEpisodeStatusChange((string identifier, PriceEpisodeStatus status) removed)
        {
            return new PriceEpisodeStatusChange
            {
                DataLock = new LegacyDataLockEvent
                {
                    PriceEpisodeIdentifier = removed.identifier,
                    Status = removed.status,
                }
            };
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
