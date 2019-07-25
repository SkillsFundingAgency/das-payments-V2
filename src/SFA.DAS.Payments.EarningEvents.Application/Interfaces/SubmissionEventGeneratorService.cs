using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Model.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface ISubmissionEventGeneratorService
    {
        Task ProcessEarningEvent(ApprenticeshipContractTypeEarningsEvent earningEvent, CancellationToken cancellationToken);
    }

    public class SubmissionEventGeneratorService : ISubmissionEventGeneratorService
    {
        private readonly ISubmittedPriceEpisodeRepository submittedPriceEpisodeRepository;
        private readonly IBulkWriter<LegacySubmissionEvent> submissionEventWriter;
        private readonly IPaymentLogger logger;
        private readonly ISubmissionEventProcessor submissionEventProcessor;

        public SubmissionEventGeneratorService(ISubmittedPriceEpisodeRepository submittedPriceEpisodeRepository, IBulkWriter<LegacySubmissionEvent> submissionEventWriter, IPaymentLogger logger, ISubmissionEventProcessor submissionEventProcessor)
        {
            this.submittedPriceEpisodeRepository = submittedPriceEpisodeRepository;
            this.submissionEventWriter = submissionEventWriter;
            this.logger = logger;
            this.submissionEventProcessor = submissionEventProcessor;
        }

        public async Task ProcessEarningEvent(ApprenticeshipContractTypeEarningsEvent earningEvent, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Processing earning event for UKPRN {earningEvent.Ukprn} LearnRefNumber {earningEvent.Learner.ReferenceNumber}");

                var lastSeenEpisodes = await submittedPriceEpisodeRepository.GetLastSubmittedPriceEpisodes(earningEvent.Ukprn, earningEvent.Learner.ReferenceNumber, cancellationToken).ConfigureAwait(false);

                var currentEpisodes = earningEvent.PriceEpisodes.Select(e => new SubmittedPriceEpisodeEntity
                {
                    Ukprn = earningEvent.Ukprn,
                    LearnRefNumber = earningEvent.Learner.ReferenceNumber,
                    PriceEpisodeIdentifier = e.Identifier,
                    IlrDetails = new IlrDetails
                    {
                        IlrFileName = earningEvent.IlrFileName,
                        AcademicYear = earningEvent.CollectionPeriod.AcademicYear.ToString(),
                        ActualStartDate = e.CourseStartDate,
                        PlannedEndDate = e.PlannedEndDate,
                        ActualEndDate = e.ActualEndDate,
                        AimSeqNumber = earningEvent.LearningAim.SequenceNumber,
                        //CommitmentId = 
                        //CompStatus = earningEvent.CompletionStatus,
                        CompletionTotalPrice = e.CompletionAmount,
                        ComponentVersionNumber = 2,
                        //EPAOrgId = earningEvent.EpaOrgId,
                        //EmployerReferenceNumber = earningEvent.EmployerReferenceNumber,
                        //FamilyName = earningEvent.FamilyName,
                        //NiNumber = earningEvent.NiNumber,
                        //GivenNames = earningEvent.GivenNames,
                        FrameworkCode = earningEvent.LearningAim.FrameworkCode,
                        PathwayCode = earningEvent.LearningAim.PathwayCode,
                        ProgrammeType = earningEvent.LearningAim.ProgrammeType,
                        StandardCode = earningEvent.LearningAim.StandardCode,
                        Uln = earningEvent.Learner.Uln,
                        FileDateTime = ExtractFileDateFromName(earningEvent.IlrFileName, earningEvent.CollectionPeriod.AcademicYear, earningEvent.IlrSubmissionDateTime),
                        SubmittedDateTime = earningEvent.IlrSubmissionDateTime,
                        //OnProgrammeTotalPrice = e.TotalNegotiatedPrice1
                    }
                });

                foreach (var currentEpisode in currentEpisodes)
                {
                    var submissionEvent = submissionEventProcessor.ProcessSubmission(currentEpisode, lastSeenEpisodes.FirstOrDefault(e => e.PriceEpisodeIdentifier == currentEpisode.PriceEpisodeIdentifier));
                    if (submissionEvent != null)
                        await submissionEventWriter.Write(submissionEvent, cancellationToken).ConfigureAwait(false);
                }

                await submissionEventWriter.Flush(cancellationToken).ConfigureAwait(false);

                logger.LogVerbose($"Processed earning event for UKPRN {earningEvent.Ukprn} LearnRefNumber {earningEvent.Learner.ReferenceNumber}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing earning event for UKPRN {earningEvent.Ukprn} LearnRefNumber {earningEvent.Learner.ReferenceNumber}. {ex.Message}");
                throw;
            }
        }

        private static DateTime ExtractFileDateFromName(string fileName, int academicYear, DateTime defaultDate) // borrowed from v1
        {
            if (string.IsNullOrEmpty(fileName) || fileName.Length < 30 || !fileName.Contains("-"))
                return defaultDate;

            var bits = fileName.Split('-');
            var index = Array.IndexOf(bits, academicYear.ToString());
            if (index >= 0 && bits.Length > index)
            {
                if (DateTime.TryParseExact(bits[index + 1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateTime))
                    return dateTime;
            }

            return defaultDate;
        }
    }
}
