using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class PriceEpisodeStatusChangeBuilder
    {
        private readonly IApprenticeshipRepository apprenticeshipRepository;
        public PriceEpisodeStatusChangeBuilder(IApprenticeshipRepository apprenticeshipRepository)
        {
            this.apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task<List<PriceEpisodeStatusChange>> Build(List<DataLockEvent> dataLockEvents,
            List<(string identifier, PriceEpisodeStatus status)> priceEpisodeChanges,
            List<PriceEpisodeStatusChange> currentPriceEpisodeStatusChange, short currentAcademicYear)
        {
            var present = await BuildPresentEvents(dataLockEvents, priceEpisodeChanges);
            
            var removed = BuildRemovedEvents(priceEpisodeChanges, currentPriceEpisodeStatusChange, currentAcademicYear);

            return present.Union(removed).ToList();
        }

        private async Task<List<PriceEpisodeStatusChange>> BuildPresentEvents(List<DataLockEvent> dataLockEvents,
            List<(string identifier, PriceEpisodeStatus status)> priceEpisodeChanges)
        {
            var events = new List<PriceEpisodeStatusChange>();
            
            foreach (var priceEpisodeChange in priceEpisodeChanges.Where(x => x.status != PriceEpisodeStatus.Removed))
            {
                var priceEpisode = dataLockEvents
                    .SelectMany(x => x.PriceEpisodes)
                    .First(p => p.Identifier == priceEpisodeChange.identifier);

                var priceEpisodeDataLocks = dataLockEvents
                    .Where(x => x.PriceEpisodes.Any(p => p.Identifier == priceEpisode.Identifier))
                    .ToList();

                var priceEpisodeErrors = priceEpisodeDataLocks
                    .OfType<EarningFailedDataLockMatching>()
                    .SelectMany(x => x.OnProgrammeEarnings)
                    .SelectMany(x => x.Periods)
                    .Where(p => p.PriceEpisodeIdentifier.Equals(priceEpisode.Identifier, StringComparison.InvariantCultureIgnoreCase))
                    .SelectMany(p => p.DataLockFailures)
                    .ToList();

                var priceEpisodeEarnings = priceEpisodeDataLocks
                    .SelectMany(x => x.OnProgrammeEarnings)
                    .Where(p => p.Periods
                        .Any(o => !string.IsNullOrWhiteSpace(o.PriceEpisodeIdentifier) &&
                                  o.PriceEpisodeIdentifier.Equals(priceEpisode.Identifier, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

                var priceEpisodeEarningPeriods = priceEpisodeDataLocks
                    .SelectMany(x => x.OnProgrammeEarnings)
                    .SelectMany(x => x.Periods)
                    .Where(p => !string.IsNullOrWhiteSpace(p.PriceEpisodeIdentifier) && 
                                p.PriceEpisodeIdentifier.Equals(priceEpisode.Identifier, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                var priceEpisodeApprenticeshipIds = new List<long>();
                priceEpisodeApprenticeshipIds.AddRange(priceEpisodeEarningPeriods
                    .Where(p => p.ApprenticeshipId.HasValue)
                    .Select(p => p.ApprenticeshipId.Value)
                    .ToList());
                priceEpisodeApprenticeshipIds.AddRange(priceEpisodeErrors
                    .Where(p => p.ApprenticeshipId.HasValue)
                    .Select(p => p.ApprenticeshipId.Value)
                    .ToList());
                
                var apprenticeships = await apprenticeshipRepository
                    .Get(priceEpisodeApprenticeshipIds.Distinct().ToList(), CancellationToken.None);
                
                foreach (var apprenticeship in apprenticeships)
                {
                    var apprenticeshipEarnings = priceEpisodeEarnings
                        .Where(o => o.Periods.Any(p =>
                            (p.ApprenticeshipId.HasValue && p.ApprenticeshipId == apprenticeship.Id) ||
                            (p.DataLockFailures != null && p.DataLockFailures.Any(d => d.ApprenticeshipId.HasValue && d.ApprenticeshipId == apprenticeship.Id))
                        ))
                        .ToList();

                    var apprenticeshipErrors = priceEpisodeErrors
                        .Where(p => p.ApprenticeshipId.HasValue && p.ApprenticeshipId == apprenticeship.Id)
                        .ToList();

                    var evt = MapPriceEpisodeStatusChange(priceEpisodeDataLocks.First(),
                        apprenticeshipErrors,
                        priceEpisodeChange.status,
                        priceEpisode,
                        apprenticeshipEarnings,
                        apprenticeship);

                    events.Add(evt);
                }
            }

            return events;
        }

        private PriceEpisodeStatusChange MapPriceEpisodeStatusChange(
            DataLockEvent dataLock,
            List<DataLockFailure> apprenticeshipErrors,
            PriceEpisodeStatus status,
            PriceEpisode priceEpisode,
            List<OnProgrammeEarning> apprenticeshipEarnings,
            ApprenticeshipModel apprenticeship)
        {
            var hasTnp3 = priceEpisode.TotalNegotiatedPrice3 > 0;
            var priceEpisodeStatusChangeId = Guid.NewGuid();
            var commitmentVersions = BuildCommitmentVersions(priceEpisodeStatusChangeId, apprenticeship);
            var periods = BuildPeriods(priceEpisodeStatusChangeId, dataLock, apprenticeshipEarnings, commitmentVersions);
            var errors = BuildCommitmentErrors(priceEpisodeStatusChangeId, apprenticeshipErrors);

            return new PriceEpisodeStatusChange
            {
                AgreedPrice = priceEpisode.AgreedPrice,
                DataLock = new LegacyDataLockEvent
                {
                    DataLockEventId = priceEpisodeStatusChangeId,
                    PriceEpisodeIdentifier = priceEpisode.Identifier,
                    Status = status,
                    AcademicYear = dataLock.CollectionPeriod.AcademicYear.ToString(),
                    UKPRN = dataLock.Ukprn,
                    EventSource = 1, // submission
                    HasErrors = apprenticeshipErrors.Any(),
                    ULN = dataLock.Learner.Uln,
                    ProcessDateTime = DateTime.UtcNow,
                    LearnRefNumber = dataLock.Learner.ReferenceNumber,
                    IlrFrameworkCode = dataLock.LearningAim.FrameworkCode,
                    IlrPathwayCode = dataLock.LearningAim.PathwayCode,
                    IlrProgrammeType = dataLock.LearningAim.ProgrammeType,
                    IlrStandardCode = dataLock.LearningAim.StandardCode,
                    SubmittedDateTime = dataLock.IlrSubmissionDateTime,

                    CommitmentId = apprenticeship.Id,
                    EmployerAccountId = apprenticeship.AccountId,

                    AimSeqNumber = dataLock.LearningAim.SequenceNumber,
                    IlrPriceEffectiveFromDate = priceEpisode.EffectiveTotalNegotiatedPriceStartDate,
                    IlrPriceEffectiveToDate = priceEpisode.ActualEndDate.GetValueOrDefault(priceEpisode.PlannedEndDate),
                    IlrEndpointAssessorPrice =
                        hasTnp3 ? priceEpisode.TotalNegotiatedPrice4 : priceEpisode.TotalNegotiatedPrice2,
                    IlrFileName = TrimUkprnFromIlrFileNameLimitToValidLength(dataLock.IlrFileName),
                    IlrStartDate = priceEpisode.CourseStartDate,
                    IlrTrainingPrice = hasTnp3 ? priceEpisode.TotalNegotiatedPrice3 : priceEpisode.TotalNegotiatedPrice1,
                },

                CommitmentVersions = commitmentVersions.ToArray(),
                Periods = periods,
                Errors = errors,
            };
        }

        private static string TrimUkprnFromIlrFileNameLimitToValidLength(string input)
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

        private static List<LegacyDataLockEventCommitmentVersion> BuildCommitmentVersions(Guid dataLockEventId,
            ApprenticeshipModel apprenticeship)
        {
            if (apprenticeship == null) return new List<LegacyDataLockEventCommitmentVersion>();

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
                .ToList();
        }

        private static LegacyDataLockEventError[] BuildCommitmentErrors(Guid eventId, List<DataLockFailure> dataLockFailures)
        {
            return dataLockFailures
                .Select(o => o.DataLockError)
                .Distinct()
                .Select(dataLockErrorCode => new LegacyDataLockEventError
                {
                    DataLockEventId = eventId,
                    SystemDescription = GetDataLockDescription(dataLockErrorCode),
                    ErrorCode = dataLockErrorCode.ToString()
                })
                .ToArray();
        }

        private static string GetDataLockDescription(DataLockErrorCode dLockCode)
        {
            switch (dLockCode)
            {
                case DataLockErrorCode.DLOCK_03:
                    return "No matching record found in the employer digital account for the standard code";
                case DataLockErrorCode.DLOCK_04:
                    return "No matching record found in the employer digital account for the framework code";
                case DataLockErrorCode.DLOCK_05:
                    return "No matching record found in the employer digital account for the programme type";
                case DataLockErrorCode.DLOCK_06:
                    return "No matching record found in the employer digital account for the pathway code";
                case DataLockErrorCode.DLOCK_07:
                    return
                        "No matching record found in the employer digital account for the negotiated cost of training";
                case DataLockErrorCode.DLOCK_08:
                    return "Multiple matching records found in the employer digital account";
                case DataLockErrorCode.DLOCK_09:
                    return
                        "The start date for this negotiated price is before the corresponding price start date in the employer digital account";
                case DataLockErrorCode.DLOCK_10: return "The employer has stopped payments for this apprentice";
                case DataLockErrorCode.DLOCK_11: return "The employer is not currently a levy payer";
                case DataLockErrorCode.DLOCK_12: return "DLOCK_12";
                default: return dLockCode.ToString();
            }
        }

        private LegacyDataLockEventPeriod[] BuildPeriods(
            Guid priceEpisodeStatusChangeId,
            DataLockEvent dataLock,
            List<OnProgrammeEarning> apprenticeshipEarnings,
            List<LegacyDataLockEventCommitmentVersion> commitmentVersions)
        {
            var eventPeriods = new List<LegacyDataLockEventPeriod>();
            var collectionPeriod = dataLock.CollectionPeriod.Period;
            var allTransactionTypeFlagGroups = apprenticeshipEarnings
                .GroupBy(o => GetTransactionTypeFlag((TransactionType) o.Type))
                .Distinct()
                .ToList();

            foreach (var allTransactionTypeFlagGroup in allTransactionTypeFlagGroups)
            {
                foreach (var commitmentVersion in commitmentVersions)
                {
                    var transactionTypeHasErrors = allTransactionTypeFlagGroup
                        .Any(x => x.Periods.Any(p => p.DataLockFailures != null &&  p.DataLockFailures.Any()));

                    eventPeriods.Add(new LegacyDataLockEventPeriod
                    {
                        DataLockEventId = priceEpisodeStatusChangeId,
                        TransactionTypesFlag = GetTransactionTypeFlag((TransactionType) allTransactionTypeFlagGroup.Key),
                        CollectionPeriodYear = dataLock.CollectionPeriod.AcademicYear,
                        CollectionPeriodName = $"{dataLock.CollectionPeriod.AcademicYear}-{collectionPeriod:D2}",
                        CollectionPeriodMonth = (collectionPeriod < 6) ? collectionPeriod + 7 : collectionPeriod - 5,
                        IsPayable = !transactionTypeHasErrors,
                        CommitmentVersion = commitmentVersion.CommitmentVersion
                    });
                }
            }

            return eventPeriods.ToArray();
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
                    throw new ArgumentException($"Transaction Type {transactionType} not supported.",
                        nameof(transactionType));
            }
        }

        private static List<PriceEpisodeStatusChange> BuildRemovedEvents(List<(string identifier,
            PriceEpisodeStatus status)> priceEpisodeChanges,
            IEnumerable<PriceEpisodeStatusChange> priceEpisodeStatusChange, short currentAcademicYear)
        {

            var removedPriceEpisodes = priceEpisodeStatusChange
                .Where(c => priceEpisodeChanges
                    .Any(p => p.status == PriceEpisodeStatus.Removed && p.identifier == c.DataLock.PriceEpisodeIdentifier))
                .ToList();

            //Ensure that only events for the current academic year can be flagged as removed.
            removedPriceEpisodes.ForEach(x => x.DataLock.Status = x.DataLock.AcademicYear.Equals(currentAcademicYear.ToString(),StringComparison.OrdinalIgnoreCase) ? PriceEpisodeStatus.Removed : PriceEpisodeStatus.Updated);

            return removedPriceEpisodes;
        }


    }
}
